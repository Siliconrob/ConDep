﻿using System.IO;
using System.Reflection;
using System.Threading;
using ConDep.Dsl;
using ConDep.Dsl.Config;
using ConDep.Dsl.Logging;
using ConDep.Dsl.Remote;
using ConDep.Dsl.Remote.Node;

namespace ConDep.Execution
{
  internal class PreRemoteOps : RemoteOperation
  {
    private const string TMP_FOLDER = @"{0}\temp\ConDep";
    private readonly PowerShellExecutor _psExecutor;

    public PreRemoteOps(PowerShellExecutor psExecutor)
    {
      _psExecutor = psExecutor;
    }

    public override string Name => "Pre-Operation";

    public override Result Execute(IOfferRemoteOperations remote, ServerConfig server, ConDepSettings settings,
      CancellationToken token)
    {
      token.ThrowIfCancellationRequested();

      Logger.WithLogSection("Pre-Operations", () =>
      {
        server.GetServerInfo().TempFolderDos = string.Format(TMP_FOLDER, "%windir%");
        Logger.Info(string.Format("Dos temp folder is {0}", server.GetServerInfo().TempFolderDos));

        server.GetServerInfo().TempFolderPowerShell = string.Format(TMP_FOLDER, "$env:windir");
        Logger.Info(string.Format("PowerShell temp folder is {0}", server.GetServerInfo().TempFolderPowerShell));

        PublishConDepNode(server, settings);

        var scriptPublisher = new PowerShellScriptPublisher(settings, server);
        Logger.WithLogSection("Copying external scripts", () => scriptPublisher.PublishScripts());
        Logger.WithLogSection("Copying remote helper assembly", () => scriptPublisher.PublishRemoteHelperAssembly());

        InstallChocolatey(server, settings);
      });
      return Result.SuccessUnChanged();
    }

    private void InstallChocolatey(ServerConfig server, ConDepSettings settings)
    {
      Logger.WithLogSection("Installing Chocolatey", () =>
      {
        _psExecutor.Execute(server, @"
try {
    if(Assert-ConDepChocoExist) {
        Invoke-ConDepChocoUpgrade
    }
    else {
        Invoke-ConDepChocoInstall
    }
}
catch {
    Write-Warning 'Failed to install Chocolatey! This could break operations depending on Chocolatey.'
    Write-Warning ""Error message: $($_.Exception.Message)""
}
");
      });
    }

    private void PublishConDepNode(ServerConfig server, ConDepSettings settings)
    {
      Logger.WithLogSection("Validating ConDepNode", () =>
      {
        string path;

        var executionPath = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location),
          "ConDepNode.exe");
        if (!File.Exists(executionPath))
        {
          var currentPath = Path.Combine(Directory.GetCurrentDirectory(), "ConDepNode.exe");
          if (!File.Exists(currentPath))
            throw new FileNotFoundException("Could not find ConDepNode.exe. Paths tried: \n" +
                                            executionPath + "\n" + currentPath);
          path = currentPath;
        }
        else
        {
          path = executionPath;
        }

        var nodeUrl = new ConDepNodeUrl(server);

        var nodePublisher = new ConDepNodePublisher(path,
          Path.Combine(server.GetServerInfo().OperatingSystem.ProgramFilesFolder, "ConDepNode", Path.GetFileName(path)),
          nodeUrl, new PowerShellExecutor());
        nodePublisher.Execute(server);
        if (!nodePublisher.ValidateNode(nodeUrl, server.DeploymentUser.UserName, server.DeploymentUser.Password,
          server))
          throw new ConDepNodeValidationException(
            "Unable to make contact with ConDep Node or return content from API.");

        Logger.Info(string.Format("ConDep Node successfully validated on {0}", server.Name));
        Logger.Info(string.Format("Node listening on {0}", nodeUrl.ListenUrl));
      });
    }
  }
}