using System.Collections.Generic;
using ConDep.Dsl;
using ConDep.Dsl.Config;

namespace ConDep.Execution
{
  internal class RunbookConfigurationHandler
  {
    private readonly ILoadBalance _loadBalancer;
    private readonly IResolveRunbookDependencies _runbookDependencyHandler;
    private readonly IDiscoverRunbooks _runbookHandler;

    public RunbookConfigurationHandler(IDiscoverRunbooks runbookHandler,
      IResolveRunbookDependencies runbookDependencyHandler, ILoadBalance loadBalancer)
    {
      _runbookHandler = runbookHandler;
      _runbookDependencyHandler = runbookDependencyHandler;
      _loadBalancer = loadBalancer;
    }

    public IEnumerable<Runbook> GetRunbooksToExecute(ConDepSettings settings)
    {
      var runbook = _runbookHandler.GetRunbook(settings);
      var dependantRunbooks = _runbookDependencyHandler.GetDependeciesForRunbook(runbook, settings);
      dependantRunbooks.Add(runbook);
      return dependantRunbooks;
    }
  }
}