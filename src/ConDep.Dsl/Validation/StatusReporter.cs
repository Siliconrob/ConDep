using System;
using System.Collections.Generic;

namespace ConDep.Dsl.Validation
{
  public class StatusReporter
  {
    private readonly List<string> _conditionMessages = new List<string>();
    private readonly List<Exception> _untrappedExceptions = new List<Exception>();

    public bool HasErrors => false;

    public bool HasExitCodeErrors => _untrappedExceptions.Count > 0;

    public void AddUntrappedException(Exception exception)
    {
      _untrappedExceptions.Add(exception);
    }

    public void AddConditionMessage(string message)
    {
      _conditionMessages.Add(message);
    }
  }
}