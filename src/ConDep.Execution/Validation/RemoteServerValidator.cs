﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using ConDep.Dsl.Config;
using ConDep.Dsl.Harvesters;
using ConDep.Dsl.Logging;
using ConDep.Dsl.Remote;
using ConDep.Dsl.Validation;

namespace ConDep.Execution.Validation
{
  public class RemoteServerValidator : IValidateServer
  {
    private readonly PowerShellExecutor _psExecutor;
    private readonly ServerInfoHarvester _serverInfoHarvester;
    private readonly IEnumerable<ServerConfig> _servers;

    public RemoteServerValidator(IEnumerable<ServerConfig> servers, ServerInfoHarvester serverInfoHarvester,
      PowerShellExecutor psExecutor)
    {
      _servers = servers;
      _serverInfoHarvester = serverInfoHarvester;
      _psExecutor = psExecutor;
    }

    public bool Validate()
    {
      return Logger.WithLogSection("Validating Servers", () =>
      {
        var isValid = true;
        foreach (var server in _servers)
        {
          var currentServer = server;
          Logger.WithLogSection(string.Format("Validating {0}", server.Name), () =>
          {
            if (!ValidateWinRm(currentServer))
            {
              isValid = false;
              return;
            }

            if (!ValidatePowerShellVersion(currentServer))
            {
              isValid = false;
              return;
            }

            Logger.WithLogSection("Collecting server data", () => _serverInfoHarvester.Harvest(currentServer));

            if (HaveNet40(currentServer))
            {
              Logger.Info("Server requirements on [{0}] are OK.", currentServer.Name);
            }
            else
            {
              Logger.Error("Server requirements on [{0}] are NOT OK.", currentServer.Name);
              isValid = false;
            }
          });
        }
        return isValid;
      });
    }

    private bool ValidatePowerShellVersion(ServerConfig currentServer)
    {
      return Logger.WithLogSection("Validating remote PowerShell version (must be 3.0 or higher)", () =>
      {
        var versionResult = _psExecutor.Execute(currentServer, "$psVersionTable.PSVersion.Major",
          mod => mod.LoadConDepModule = false, logOutput: false);
        if (versionResult == null)
        {
          Logger.Error("Unable to get remote PowerShell version.");
          return false;
        }

        var version = versionResult.First();

        Logger.Info(string.Format("Remote PowerShell version is {0}", version));
        return version >= 3;
      });
    }

    private static bool ValidateWinRm(ServerConfig server)
    {
      return Logger.WithLogSection("Validating WinRM", () =>
      {
        Logger.Info(string.Format("Checking for WinRM (PowerShell Remoting) on [{0}]...",
          server.Name));

        if (HaveAccessToServer(server))
        {
          Logger.Info(string.Format("Successfully validated WinRM on [{0}].", server.Name));
        }
        else
        {
          Logger.Error(
            string.Format("Could not connect to remote server [{0}] with WinRM (PowerShell Remoting).",
              server.Name));
          Logger.Error(string.Format("Server requirements on [{0}] are NOT OK.", server.Name));
          return false;
        }
        return true;
      });
    }

    private static bool HaveAccessToServer(ServerConfig server)
    {
      Logger.Info(
        string.Format("Checking if WinRM (Remote PowerShell) can be used to reach remote server [{0}]...",
          server.Name));
      var cmd = server.DeploymentUser.IsDefined()
        ? string.Format("id -r:{0} -u:{1} -p:\"{2}\"", server.Name,
          server.DeploymentUser.UserName, server.DeploymentUser.Password)
        : string.Format("id -r:{0}", server.Name);
      if (server.PowerShell.SSL)
      {
        Logger.Info(string.Format("Using SSL via WinRM to reach remote server [{0}]...", server.Name));
        cmd = string.Concat(cmd, " -usessl");
      }

      var success = false;
      var path = Environment.ExpandEnvironmentVariables(@"%windir%\system32\WinRM.cmd");
      var startInfo = new ProcessStartInfo(path)
      {
        Arguments = cmd,
        Verb = "RunAs",
        UseShellExecute = false,
        WindowStyle = ProcessWindowStyle.Hidden,
        RedirectStandardError = true,
        RedirectStandardOutput = true
      };
      var process = Process.Start(startInfo);
      process.WaitForExit();

      if (process.ExitCode == 0)
      {
        var message = process.StandardOutput.ReadToEnd();
        Logger.Info(string.Format("Contact was made with server [{0}] using WinRM (Remote PowerShell). ",
          server.Name));
        Logger.Verbose(string.Format("Details: {0} ", message));
        success = true;
      }
      else
      {
        var errorMessage = process.StandardError.ReadToEnd();
        Logger.Error(string.Format("Unable to reach server [{0}] using WinRM (Remote PowerShell)",
          server.Name));
        Logger.Error(string.Format("Details: {0}", errorMessage));
      }
      return success;
    }

    private static bool HaveNet40(ServerConfig server)
    {
      Logger.Info(string.Format("Checking if .NET Framework 4.0 is installed on server [{0}]...", server.Name));
      var success = server.GetServerInfo().DotNetFrameworks.HasVersion(DotNetVersion.v4_0_full);

      if (success)
        Logger.Info(string.Format("Microsoft .NET Framework version 4.0 is installed on server [{0}].",
          server.Name));
      else
        Logger.Error(string.Format("Missing Microsoft .NET Framework version 4.0 on [{0}].", server.Name));
      return success;
    }
  }
}