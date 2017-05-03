using System;
using ConDep.Dsl.LoadBalancer;

namespace ConDep.Dsl.Config
{
  [Serializable]
  public class ServerConfig
  {
    private readonly ServerInfo _serverInfo = new ServerInfo();
    private DeploymentUserConfig _deploymentUserRemote;

    public string Name { get; set; }
    public bool StopServer { get; set; }

    public DeploymentUserConfig DeploymentUser
    {
      get => _deploymentUserRemote ?? (_deploymentUserRemote = new DeploymentUserConfig());
      set => _deploymentUserRemote = value;
    }

    public string LoadBalancerFarm { get; set; }
    public ServerLoadBalancerState LoadBalancerState { get; } = new ServerLoadBalancerState();

    public PowerShellConfig PowerShell { get; set; }

    public NodeConfig Node { get; set; }

    public ServerInfo GetServerInfo()
    {
      return _serverInfo;
    }
  }
}