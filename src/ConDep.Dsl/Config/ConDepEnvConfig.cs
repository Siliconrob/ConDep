﻿using System;
using System.Collections.Generic;
using System.Linq;

namespace ConDep.Dsl.Config
{
  [Serializable]
  public class ConDepEnvConfig
  {
    private DeploymentUserConfig _deploymentUserRemote;
    private NodeConfig _node = new NodeConfig();
    private PowerShellConfig _powerShell = new PowerShellConfig();
    private string[] _powerShellScriptFolders = new string[0];
    public string EnvironmentName { get; set; }

    public string[] PowerShellScriptFolders
    {
      get => _powerShellScriptFolders;
      set => _powerShellScriptFolders = value;
    }

    public LoadBalancerConfig LoadBalancer { get; set; }
    public IList<ServerConfig> Servers { get; set; }
    public IList<TiersConfig> Tiers { get; set; }

    public DeploymentUserConfig DeploymentUser
    {
      get => _deploymentUserRemote ?? (_deploymentUserRemote = new DeploymentUserConfig());
      set => _deploymentUserRemote = value;
    }

    public dynamic OperationsConfig { get; set; }
    public bool UsingTiers => Tiers != null && Tiers.Count > 0;

    public NodeConfig Node
    {
      get => _node;
      set => _node = value;
    }

    public PowerShellConfig PowerShell
    {
      get => _powerShell;
      set => _powerShell = value;
    }

    public void AddServer(string name)
    {
      Servers.Add(new ServerConfig
      {
        Name = name,
        DeploymentUser = DeploymentUser,
        Node = new NodeConfig {Port = 4444, TimeoutInSeconds = 100},
        PowerShell = new PowerShellConfig {HttpPort = 5985, HttpsPort = 5986},
        StopServer = false
      });
    }
  }

  public static class TiersExentsions
  {
    public static bool Exists(this IList<TiersConfig> tiers, string tierName)
    {
      return tiers.SingleOrDefault(x => x.Name.Equals(tierName, StringComparison.OrdinalIgnoreCase)) != null;
    }
  }

  [Serializable]
  public class PowerShellConfig
  {
    public int? HttpPort { get; set; }
    public int? HttpsPort { get; set; }
    public bool SSL { get; set; }
  }

  [Serializable]
  public class NodeConfig
  {
    //private int _port = 4444;
    //private int _timeoutInSeconds = 100;

    public int? Port { get; set; }

    public int? TimeoutInSeconds { get; set; }
  }
}