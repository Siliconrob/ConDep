﻿using System;
using System.IO;
using NDesk.Options;

namespace ConDep.Console.Help
{
  public class CmdHelpParser : CmdBaseParser<ConDepHelpOptions>
  {
    public CmdHelpParser(string[] args) : base(args)
    {
      OptionSet = new OptionSet();
    }

    public override OptionSet OptionSet { get; }

    public override ConDepHelpOptions Parse()
    {
      var options = new ConDepHelpOptions();
      if (_args == null || _args.Length == 0)
        return options;

      var command = _args[0].Trim().ToLower();
      if (command == "deploy")
        options.Command = ConDepCommand.Deploy;
      else if (command == "encrypt")
        options.Command = ConDepCommand.Encrypt;
      else if (command == "decrypt")
        options.Command = ConDepCommand.Decrypt;
      else if (command == "relay")
        options.Command = ConDepCommand.Relay;
      else if (command == "server")
        options.Command = ConDepCommand.Server;
      else
        throw new ConDepCmdParseException(
          $"The command [{command}] is unknown to ConDep and unable to show help for command.");
      return options;
    }

    public override void WriteOptionsHelp(TextWriter writer)
    {
      throw new NotImplementedException();
    }
  }
}