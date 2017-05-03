using System;
using System.Collections.Generic;
using System.Linq;
using ConDep.Dsl.Config;
using ConDep.Dsl.Logging;

namespace ConDep.Dsl.Harvesters
{
  public class ServerInfoHarvester
  {
    private readonly IEnumerable<Type> _mandatoryHarversters;
    private readonly ConDepSettings _settings;
    private List<IHarvestServerInfo> _harvesters;

    public ServerInfoHarvester(ConDepSettings settings, IEnumerable<Type> mandatoryHarversters)
    {
      _settings = settings;
      _mandatoryHarversters = mandatoryHarversters;
    }

    private List<IHarvestServerInfo> Harvesters => _harvesters ?? (_harvesters = GetHarvesters(_settings).ToList());

    public void Harvest(ServerConfig server)
    {
      Harvesters.ForEach(x => Logger.WithLogSection(x.GetType().Name, () => x.Harvest(server)));
    }

    private IEnumerable<IHarvestServerInfo> GetHarvesters(ConDepSettings settings)
    {
      if (settings.Options.SkipHarvesting)
        return GetMandatoryHarvesters(GetType().Assembly.GetTypes());

      var internalHarvesters = GetHarvesters(GetType().Assembly.GetTypes());
      if (settings.Options.Assembly == null)
        return internalHarvesters;

      var externalHarvesters = GetHarvesters(settings.Options.Assembly.GetTypes());
      return internalHarvesters.Concat(externalHarvesters);
    }

    private IEnumerable<IHarvestServerInfo> GetHarvesters(IEnumerable<Type> types)
    {
      return types.Where(t => t.GetInterfaces().Contains(typeof(IHarvestServerInfo)))
        .Select(t => Activator.CreateInstance(t) as IHarvestServerInfo);
    }

    private IEnumerable<IHarvestServerInfo> GetMandatoryHarvesters(IEnumerable<Type> types)
    {
      return types.Where(t => _mandatoryHarversters.Any(x => x == t))
        .Select(t => Activator.CreateInstance(t) as IHarvestServerInfo);
    }
  }
}