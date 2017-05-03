using System;
using System.Linq;
using ConDep.Dsl.Config;
using ConDep.Dsl.Remote;

namespace ConDep.Dsl.Harvesters
{
  internal class NetworkHarvester : IHarvestServerInfo
  {
    private readonly PowerShellExecutor _executor;

    public NetworkHarvester()
    {
      _executor = new PowerShellExecutor();
    }

    public NetworkHarvester(PowerShellExecutor executor)
    {
      _executor = executor;
    }

    public void Harvest(ServerConfig server)
    {
      var networkInfo = @"$result = @()
$networkInterfaces = Get-WmiObject win32_networkadapterconfiguration | where { $_.IPEnabled }
foreach($interface in $networkInterfaces) {
    $ifaceInfo = @{}
    $ifaceInfo.IPAddresses = $interface.IPAddress
    $ifaceInfo.IPSubnets = $interface.IPSubnet
    $ifaceInfo.Description = $interface.Description
    $ifaceInfo.DefaultGateways = $interface.DefaultIPGateway
    $ifaceInfo.DHCPEnabled = $interface.DHCPEnabled
    $ifaceInfo.DNSDomain = $interface.DNSDomain
    $ifaceInfo.DNSHostName = $interface.DNSHostName
    $ifaceInfo.Index = $interface.Index
    $ifaceInfo.InterfaceIndex = $interface.InterfaceIndex

    $result += ,@($ifaceInfo)
}

return $result";

      var networkInfoResult = _executor.Execute(server, networkInfo, mod => mod.LoadConDepModule = false,
        logOutput: false);
      if (networkInfoResult != null)
        foreach (var network in networkInfoResult)
        {
          //object[] ipAddresses = network.IPAddresses;
          //IEnumerable<string> ipAddresses2 = ipAddresses.Cast<string>();
          //object[] gateways = network.DefaultGateways;

          var info = new NetworkInfo
          {
            Description = network.Description,
            DHCPEnabled = network.DHCPEnabled,
            DNSDomain = network.DNSDomain,
            DNSHostName = network.DNSHostName,
            Index = Convert.ToInt32(network.Index),
            InterfaceIndex = Convert.ToInt32(network.InterfaceIndex)
          };

          if (network.IPAddresses is string)
          {
            info.IPAddresses = new string[] {network.IPAddresses};
          }
          else
          {
            object[] addresses = network.IPAddresses;
            info.IPAddresses = addresses.Cast<string>();
          }

          if (network.IPSubnets is string)
          {
            info.IPSubnets = new string[] {network.IPSubnets};
          }
          else
          {
            object[] subnets = network.IPSubnets;
            info.IPSubnets = subnets.Cast<string>();
          }

          if (network.DefaultGateways != null)
            if (network.DefaultGateways is string)
            {
              info.DefaultGateways = new string[] {network.DefaultGateways};
            }
            else
            {
              object[] gateways = network.DefaultGateways;
              info.IPAddresses = gateways.Cast<string>();
            }

          server.GetServerInfo().Network.Add(info);
        }
    }
  }
}