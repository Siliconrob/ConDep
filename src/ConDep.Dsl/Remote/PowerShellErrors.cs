using System;
using System.Collections.Generic;
using System.Linq;

namespace ConDep.Dsl.Remote
{
  public class PowerShellErrors : Exception
  {
    private readonly List<Exception> _exceptions = new List<Exception>();

    public override string Message
    {
      get
      {
        if (_exceptions.Count > 1)
        {
          var counter = 1;
          return _exceptions.Aggregate("",
            (current, exception) =>
              current +
              string.Format("Exception #{0}: " + exception + "\n", counter++));
        }
        return _exceptions.Aggregate("", (current, exception) => current + exception);
      }
    }

    public void Add(Exception exception)
    {
      _exceptions.Add(exception);
    }

    public override string ToString()
    {
      return Message;
    }
  }
}