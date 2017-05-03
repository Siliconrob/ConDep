using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Management.Automation;
using System.Management.Automation.Runspaces;
using System.Text.RegularExpressions;
using System.Threading;
using ConDep.Dsl.Config;
using ConDep.Dsl.Logging;
using ConDep.Dsl.Remote;
using ConDep.Dsl.Remote.Node;
using ConDep.Execution.PSScripts.ConDepNode;
using ConDep.Execution.Resources;

namespace ConDep.Execution
{
  public class ConDepNodePublisher
  {
    private readonly string _destPath;
    private readonly PowerShellExecutor _psExecutor;
    private readonly string _srcPath;
    private readonly ConDepNodeUrl _url;

    public ConDepNodePublisher(string srcPath, string destPath, ConDepNodeUrl url, PowerShellExecutor psExecutor)
    {
      _srcPath = srcPath;
      _destPath = destPath;
      _url = url;
      _psExecutor = psExecutor;

      //_psExecutor.LoadConDepNodeModule = true;
      //_psExecutor.LoadConDepModule = false;
    }

    public void Execute(ServerConfig server)
    {
      DeployNodeModuleScript(server);
      var nodeState = GetNodeState(server);

      Logger.WithLogSection("Node State", () =>
      {
        Logger.Info("Running : " + nodeState.IsNodeServiceRunning);
        Logger.Info("Need Update : " + nodeState.NeedNodeDeployment);
      });
      if (nodeState.NeedNodeDeployment)
        DeployNode(server);
      else if (!nodeState.IsNodeServiceRunning)
        StartNode(server);
    }

    private void DeployNode(ServerConfig server)
    {
      var byteArray = File.ReadAllBytes(_srcPath);
      var parameters = new List<CommandParameter>
      {
        new CommandParameter("path", _destPath),
        new CommandParameter("data", byteArray),
        new CommandParameter("url", _url.ListenUrl),
        new CommandParameter("port", _url.Port)
      };

      _psExecutor.Execute(server, "Param([string]$path, $data, $url, $port)\n  Add-ConDepNode $path $data $url $port",
        mod =>
        {
          mod.LoadConDepNodeModule = true;
          mod.LoadConDepModule = false;
        }, parameters,
        false);
    }

    private dynamic GetNodeState(ServerConfig server)
    {
      var nodeCheckExecutor = new PowerShellExecutor();
      var nodeCheckResult =
        nodeCheckExecutor.Execute(server,
          string.Format("Get-ConDepNodeState \"{0}\" \"{1}\"", _destPath, FileHashGenerator.GetFileHash(_srcPath)),
          mod =>
          {
            mod.LoadConDepModule = false;
            mod.LoadConDepNodeModule = true;
          },
          logOutput: true);

      return nodeCheckResult.Single(psObject => psObject.ConDepResult != null).ConDepResult;
    }

    public void StartNode(ServerConfig server)
    {
      _psExecutor.Execute(server, "Start-ConDepNode", mod =>
      {
        mod.LoadConDepNodeModule = true;
        mod.LoadConDepModule = false;
      }, logOutput: false);
    }

    public bool ValidateNode(ConDepNodeUrl url, string userName, string password, ServerConfig server)
    {
      var api = new Api(url, userName, password, server.Node.TimeoutInSeconds.Value * 1000);
      if (!api.Validate())
      {
        Thread.Sleep(1000);
        return api.Validate();
      }
      return true;
    }

    private void DeployNodeModuleScript(ServerConfig server)
    {
      var resource = ConDepNodeResources.ConDepNodeModule;

      var localModulePath = GetFilePathForConDepScriptModule(resource);
      if (NeedToDeployScript(server, localModulePath))
      {
        Logger.Verbose("Found script {0} in assembly {1}", localModulePath, GetType().Assembly.FullName);
        var dstPath = Path.Combine(server.GetServerInfo().ConDepNodeScriptsFolder, Path.GetFileName(localModulePath));
        PublishFile(localModulePath, dstPath, server);
      }
    }

    private void PublishFile(string srcPath, string dstPath, ServerConfig server)
    {
      PublishFile(File.ReadAllBytes(srcPath), dstPath, server);
    }

    private void PublishFile(byte[] srcBytes, string dstPath, ServerConfig server)
    {
      const string publishScript = @"Param([string]$path, $data)
    $path = $ExecutionContext.InvokeCommand.ExpandString($path)
    $dir = Split-Path $path

    $dirInfo = [IO.Directory]::CreateDirectory($dir)
    if(Test-Path $path) {
        [IO.File]::Delete($path)
    }

    [IO.FileStream]$filestream = [IO.File]::OpenWrite( $path )
    $filestream.Write( $data, 0, $data.Length )
    $filestream.Close()
    write-host ""File $path created""
";

      var scriptParameters = new List<CommandParameter>
      {
        new CommandParameter("path", dstPath),
        new CommandParameter("data", srcBytes)
      };
      _psExecutor.Execute(server, publishScript, mod => mod.LoadConDepModule = false, scriptParameters, false);
    }

    private bool NeedToDeployScript(ServerConfig server, string localFile)
    {
      const string script = @"Param($fileWithHash, $dir)
$dir = $ExecutionContext.InvokeCommand.ExpandString($dir)

$conDepReturnValues = New-Object PSObject -Property @{         
    ConDepResult    = New-Object PSObject -Property @{
		Files = $null
    }                 
}                  

function Get-ConDepFileHash($path) {
    if(Test-Path $path) {
        $md5 = [System.Security.Cryptography.MD5]::Create()
        $hash = [System.BitConverter]::ToString($md5.ComputeHash([System.IO.File]::ReadAllBytes($path)))
        return $hash.Replace(""-"", """")
    }
    else {
        return """"
    }
}

$returnValues = @()

$hash = Get-ConDepFileHash (Join-Path -path $dir -childpath $($fileWithHash.Item1))
$returnValues += @{
	FileName = $fileWithHash.Item1
	IsEqual = ($hash -eq $fileWithHash.Item2)
}

$conDepReturnValues.ConDepResult.Files = $returnValues
return $conDepReturnValues
";

      var scriptParameters = new List<CommandParameter>
      {
        new CommandParameter("fileWithHash",
          new Tuple<string, string>(Path.GetFileName(localFile), FileHashGenerator.GetFileHash(localFile))),
        new CommandParameter("dir", server.GetServerInfo().ConDepNodeScriptsFolder)
      };

      var scriptResult = _psExecutor.Execute(server, script, opt => opt.LoadConDepModule = false, logOutput: false,
        parameters: scriptParameters);

      foreach (var psObject in scriptResult)
      {
        if (psObject.ConDepResult == null || psObject.ConDepResult.Files == null) continue;

        var remoteFilesArray = ((PSObject) psObject.ConDepResult.Files).BaseObject as ArrayList;
        var remoteFiles = remoteFilesArray.Cast<dynamic>().Select(remoteFile => remoteFile);

        return remoteFiles.Any(remoteFile => !remoteFile.IsEqual && remoteFile.FileName == Path.GetFileName(localFile));
      }

      return false;
    }

    private string GetFilePathForConDepScriptModule(string resource)
    {
      var regex = new Regex(@".+\.(.+\.(ps1|psm1))");
      var match = regex.Match(resource);
      if (match.Success)
      {
        var resourceName = match.Groups[1].Value;
        if (!string.IsNullOrWhiteSpace(resourceName))
        {
          var resourceNamespace = resource.Replace("." + resourceName, "");
          return ConDepResourceFiles.GetFilePath(GetType().Assembly, resourceNamespace, resourceName,
            keepOriginalFileName: true);
        }
      }
      return null;
    }
  }
}