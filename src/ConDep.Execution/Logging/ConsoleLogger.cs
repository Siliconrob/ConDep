using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using ConDep.Dsl.Logging;
using log4net;

namespace ConDep.Execution.Logging
{
  public class ConsoleLogger : Log4NetLoggerBase
  {
    private const string LEVEL_INDICATOR = " ";
    private readonly bool _isConsole;
    private int _indentLevel;

    public ConsoleLogger(ILog log) : base(log)
    {
      try
      {
        var bw = Console.BufferWidth;
        _isConsole = true;
      }
      catch
      {
        _isConsole = false;
      }
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

    public override void Log(string message, TraceLevel traceLevel, params object[] formatArgs)
    {
      Log(message, null, traceLevel, formatArgs);
    }

    public override void Progress(string message, params object[] formatArgs)
    {
      var prefix = GetSectionPrefix();
      var messageFormatted = TrimProgressMessage("\r               " + prefix + message + "              ", "");
      Console.Write(messageFormatted);
    }

    public override void ProgressEnd()
    {
      Console.WriteLine();
    }

    public override void Log(string message, Exception ex, TraceLevel traceLevel, params object[] formatArgs)
    {
      var lines = message.Split(new[] {"\r\n", "\n"}, StringSplitOptions.None);
      var prefix = GetSectionPrefix();
      foreach (var inlineMessage in lines)
      {
        var splitMessages = SplitMessagesByWindowSize(inlineMessage, prefix);
        foreach (var splitMessage in splitMessages)
          base.Log(prefix + splitMessage, ex, traceLevel, formatArgs);
      }
    }

    private IEnumerable<string> SplitMessagesByWindowSize(string message, string prefix)
    {
      if (!_isConsole) return new[] {message};

      var chunkSize = Console.BufferWidth - 16 - prefix.Length;
      if (message.Length <= chunkSize)
        return new[] {message};

      return Chunk(message, chunkSize);
    }

    private string TrimProgressMessage(string message, string prefix)
    {
      if (!_isConsole) return message;

      var chunkSize = Console.BufferWidth - prefix.Length;
      if (message.Length <= chunkSize)
        return message;

      return Chunk(message, chunkSize).First();
    }

    private static IEnumerable<string> Chunk(string str, int chunkSize)
    {
      for (var i = 0; i < str.Length; i += chunkSize)
        if (i + chunkSize > str.Length)
          yield return str.Substring(i);
        else
          yield return str.Substring(i, chunkSize);
    }

    private string GetSectionPrefix()
    {
      var prefix = "";
      for (var i = 0; i < _indentLevel; i++)
        prefix += LEVEL_INDICATOR;
      return prefix;
    }
  }
}