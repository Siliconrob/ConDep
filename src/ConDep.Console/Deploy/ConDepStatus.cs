using System;
using ConDep.Dsl.Logging;

namespace ConDep.Console.Deploy
{
  public class ConDepStatus
  {
    public ConDepStatus()
    {
      StartTime = DateTime.Now;
    }

    public DateTime StartTime { get; }

    public DateTime EndTime { get; set; }

    public void PrintSummary()
    {
      if (EndTime < StartTime)
        EndTime = DateTime.Now;

      var message = $@"
Start Time      : {StartTime.ToLongTimeString()}
End time        : {EndTime.ToLongTimeString()}
Time Taken      : {(EndTime - StartTime).ToString(@"%h' hrs '%m' min '%s' sec'")}
";
      Logger.Info("\n");
      Logger.WithLogSection("Summary", () => Logger.Info(message));
    }
  }
}