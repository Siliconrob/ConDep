using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using ConDep.Dsl.Logging;

namespace ConDep.Execution
{
  public class RelayApiLogger : LoggerBase, IDisposable
  {
    private const string LEVEL_INDICATOR = " ";
    private bool _disposed;
    private int _indentLevel;
    private Stream _internalLog;
    private StreamWriter _writer;

    public RelayApiLogger(Guid executionId)
    {
      var path = Path.Combine(Path.GetTempPath(), "ConDepRelay");
      Directory.CreateDirectory(path);

      _internalLog = new FileStream(Path.Combine(path, executionId + ".log"), FileMode.Append, FileAccess.Write,
        FileShare.ReadWrite);
      _writer = new StreamWriter(_internalLog);
    }

    public override TraceLevel TraceLevel { get; set; }

    public void Dispose()
    {
      Dispose(true);
      GC.SuppressFinalize(this);
    }

    public override void Progress(string message, params object[] formatArgs)
    {
    }

    public override void ProgressEnd()
    {
    }

    public override void LogSectionStart(string name)
    {
      var sectionName = _indentLevel == 0 ? name : "" + name;
      base.Log(sectionName, TraceLevel.Info);
      _indentLevel++;
    }

    public override void LogSectionEnd(string name)
    {
      _indentLevel--;
    }

    public override void Log(string message, Exception ex, TraceLevel traceLevel, params object[] formatArgs)
    {
      if (traceLevel > TraceLevel) return;

      var formattedMessage = formatArgs != null && formatArgs.Length > 0 ? string.Format(message, formatArgs) : message;
      var lines = ReformatWithPrefix(formattedMessage, traceLevel);

      foreach (var inlineMessage in lines)
        if (formatArgs != null && formatArgs.Length > 0)
          _writer.WriteLine(inlineMessage, formatArgs);
        else
          _writer.WriteLine(inlineMessage);

      if (ex != null)
      {
        _writer.WriteLine(GetSectionPrefix(traceLevel) + "Exception:");
        lines = ReformatWithPrefix(ex.Message, traceLevel);
        foreach (var inlineMessage in lines)
          _writer.WriteLine(inlineMessage);

        lines = ReformatWithPrefix(ex.StackTrace, traceLevel);
        foreach (var inlineMessage in lines)
          _writer.WriteLine(inlineMessage);

        if (ex.InnerException != null)
        {
          _writer.Flush();

          Error("Inner exception:", ex.InnerException);
        }
      }
      _writer.Flush();
    }

    private IEnumerable<string> ReformatWithPrefix(string stackTrace, TraceLevel traceLevel)
    {
      var prefix = GetSectionPrefix(traceLevel);
      return stackTrace.Split(new[] {"\r\n", "\n"}, StringSplitOptions.None).Select(line => prefix + line);
    }

    private string GetSectionPrefix(TraceLevel traceLevel)
    {
      const int fixedLengthTraceLevel = 7;
      var postFixLength = fixedLengthTraceLevel - traceLevel.ToString().Length;
      var strTrcLvl = traceLevel.ToString().PadRight(postFixLength);

      var prefix = string.Format("[{0}] ", strTrcLvl);
      for (var i = 0; i < _indentLevel; i++)
        prefix += LEVEL_INDICATOR;
      return prefix;
    }

    protected virtual void Dispose(bool disposing)
    {
      if (_disposed)
        return;

      if (disposing)
      {
        if (_writer != null)
        {
          _writer.Dispose();
          _writer = null;
        }

        if (_internalLog != null)
        {
          _internalLog.Dispose();
          _internalLog = null;
        }
      }

      _disposed = true;
    }
  }
}