using System.ServiceProcess;
using ConDep.Dsl.Logging;
using log4net;

namespace ConDep.Execution.Logging
{
  public class LogResolver : IResolveLogger
  {
    private bool? _tcServiceExist;

    private bool RunningOnTeamCity
    {
      get
      {
        if (_tcServiceExist == null)
          try
          {
            var tcService = new ServiceController("TCBuildAgent");
            _tcServiceExist = tcService.Status == ServiceControllerStatus.Running;
          }
          catch
          {
            _tcServiceExist = false;
          }
        return _tcServiceExist.Value;
      }
    }

    public ILogForConDep GetLogger()
    {
      if (RunningOnTeamCity)
        return new TeamCityLogger(LogManager.GetLogger("condep-teamcity"));
      return new ConsoleLogger(LogManager.GetLogger("condep-default"));
    }
  }
}