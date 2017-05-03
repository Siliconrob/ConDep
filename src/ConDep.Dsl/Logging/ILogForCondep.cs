using System;
using System.Diagnostics;

namespace ConDep.Dsl.Logging
{
  public interface ILogForConDep
  {
    TraceLevel TraceLevel { get; set; }
    void Warn(string message, params object[] formatArgs);
    void Warn(string message, Exception ex, params object[] formatArgs);
    void Verbose(string message, params object[] formatArgs);
    void Verbose(string message, Exception ex, params object[] formatArgs);
    void Info(string message, params object[] formatArgs);
    void Info(string message, Exception ex, params object[] formatArgs);
    void Log(string message, TraceLevel traceLevel, params object[] formatArgs);
    void Log(string message, Exception ex, TraceLevel traceLevel, params object[] formatArgs);
    void Error(string message, params object[] formatArgs);
    void Error(string message, Exception ex, params object[] formatArgs);
    void Progress(string message, params object[] formatArgs);
    void ProgressEnd();
    void LogSectionStart(string name);
    void LogSectionEnd(string name);
  }
}