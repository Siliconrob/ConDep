using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Management.Automation;
using System.Management.Automation.Host;
using System.Security;
using ConDep.Dsl.Logging;

namespace ConDep.Dsl.Remote
{
  internal class ConDepPSHostUI : PSHostUserInterface
  {
    private readonly ConDepPsHostRawUI _rawUi;

    public ConDepPSHostUI()
    {
      _rawUi = new ConDepPsHostRawUI();
    }

    public override PSHostRawUserInterface RawUI => _rawUi;

    public override string ReadLine()
    {
      throw new NotImplementedException();
    }

    public override SecureString ReadLineAsSecureString()
    {
      throw new NotImplementedException();
    }

    public override void Write(string value)
    {
      if (value == "\n") return;
      Logger.Info(value);
    }

    public override void Write(ConsoleColor foregroundColor, ConsoleColor backgroundColor, string value)
    {
      if (value == "\n") return;
      Logger.Info(value);
    }

    public override void WriteLine(string value)
    {
      Logger.Info(value);
    }

    public override void WriteErrorLine(string value)
    {
      Logger.Error(value);
    }

    public override void WriteDebugLine(string message)
    {
      Logger.Verbose(message);
    }

    public override void WriteProgress(long sourceId, ProgressRecord record)
    {
      if (record.RecordType == ProgressRecordType.Completed)
        CompleteProgress(record);
      if (record.RecordType == ProgressRecordType.Processing)
        if (record.PercentComplete > -1)
        {
          var progressIndicator = new string('>', record.PercentComplete / 5);
          var progressRemaining = new string(' ', 20 - progressIndicator.Length);
          Logger.Progress("Progress: [" + progressIndicator + progressRemaining + "]");
        }
        else if (record.StatusDescription.Length > 10)
        {
          Logger.Progress(record.StatusDescription);
        }
        else
        {
          Logger.Progress("Progress: " + record.StatusDescription);
        }
    }

    private static void CompleteProgress(ProgressRecord record)
    {
      if (record.PercentComplete > -1)
      {
        var progressIndicator = new string('>', 20);
        Logger.Progress("Progress: [" + progressIndicator + "]");
      }
      Logger.ProgressEnd();
    }

    public override void WriteVerboseLine(string message)
    {
      Logger.Verbose(message);
    }

    public override void WriteWarningLine(string message)
    {
      Logger.Warn(message);
    }

    public override Dictionary<string, PSObject> Prompt(string caption, string message,
      Collection<FieldDescription> descriptions)
    {
      return null;
    }

    public override PSCredential PromptForCredential(string caption, string message, string userName, string targetName)
    {
      throw new NotImplementedException();
    }

    public override PSCredential PromptForCredential(string caption, string message, string userName, string targetName,
      PSCredentialTypes allowedCredentialTypes, PSCredentialUIOptions options)
    {
      throw new NotImplementedException();
    }

    public override int PromptForChoice(string caption, string message, Collection<ChoiceDescription> choices,
      int defaultChoice)
    {
      return defaultChoice;
    }
  }
}