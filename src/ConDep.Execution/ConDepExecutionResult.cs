using System;
using System.Collections.Generic;

namespace ConDep.Execution
{
  [Serializable]
  public class ConDepExecutionResult
  {
    public ConDepExecutionResult(bool success)
    {
      Success = success;
    }

    public bool Success { get; }

    public bool Cancelled { get; set; }

    public List<TimedException> ExceptionMessages { get; } = new List<TimedException>();

    public void AddException(Exception exception)
    {
      ExceptionMessages.Add(new TimedException {DateTime = DateTime.UtcNow, Exception = exception});
    }

    public bool HasExceptions()
    {
      return ExceptionMessages.Count > 0;
    }
  }
}