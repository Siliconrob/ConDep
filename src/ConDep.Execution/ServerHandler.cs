﻿using System;
using System.Collections.Generic;
using System.Linq;
using ConDep.Dsl;
using ConDep.Dsl.Config;

namespace ConDep.Execution
{
  internal class ServerHandler : IDiscoverServers
  {
    public IEnumerable<ServerConfig> GetServers(Runbook runbook, ConDepSettings settings)
    {
      if (settings.Config.UsingTiers)
      {
        ValidateApplicationTier(runbook, settings);

        var tier = runbook.GetType().GetCustomAttributes(typeof(TierAttribute), false).Single() as TierAttribute;
        if (!settings.Config.Tiers.Exists(tier.TierName))
          throw new ConDepTierDoesNotExistInConfigException(string.Format("Tier {0} does not exist in {1}.env.config",
            tier.TierName, settings.Options.Environment));
        return
          settings.Config.Tiers.Single(x => x.Name.Equals(tier.TierName, StringComparison.OrdinalIgnoreCase))
            .Servers;
      }
      return settings.Config.Servers;
    }

    private static void ValidateApplicationTier(Runbook runbook, ConDepSettings settings)
    {
      var hasTier = runbook.GetType().GetCustomAttributes(typeof(TierAttribute), false).Any();
      if (!hasTier) throw new ConDepNoRunbookTierDefinedException(runbook, settings);

      var hasSingleTier = runbook.GetType().GetCustomAttributes(typeof(TierAttribute), false).SingleOrDefault() != null;
      if (!hasSingleTier)
        throw new ConDepNoRunbookTierDefinedException(
          string.Format("Multiple tiers defined for {0}. Only one tier is allowed by Artifact.",
            runbook.GetType().Name));
    }
  }
}