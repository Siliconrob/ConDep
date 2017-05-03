using System;
using System.Linq;
using ConDep.Dsl;
using ConDep.Dsl.Config;

namespace ConDep.Execution
{
  public class ConDepNoRunbookTierDefinedException : Exception
  {
    public ConDepNoRunbookTierDefinedException(string message) : base(message)
    {
    }

    public ConDepNoRunbookTierDefinedException(Runbook application, ConDepSettings settings)
      : base(string.Format(
        "No Tiers defined for application {0}. You need to specify a tier using the {1} attribute on the {0} class. Tiers available in your configuration are {2}.",
        application.GetType().Name,
        typeof(TierAttribute).Name,
        string.Join(", ", settings.Config.Tiers.Select(x => x.Name))))
    {
    }
  }
}