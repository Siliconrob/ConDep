namespace ConDep.Console.Help
{
  public class CmdHelpHandler : IHandleConDepCommands
  {
    private readonly CmdHelpParser _parser;
    private readonly CmdHelpValidator _validator;

    public CmdHelpHandler(string[] args)
    {
      _parser = new CmdHelpParser(args);
      _validator = new CmdHelpValidator();
    }

    public void Execute(CmdHelpWriter helpWriter)
    {
      HelpWriter = helpWriter;
      var options = _parser.Parse();
      _validator.Validate(options);

      if (options.NoOptions())
        helpWriter.WriteHelp();
      else
        helpWriter.WriteHelpForCommand(options.Command);
    }

    public void WriteHelp()
    {
      HelpWriter.WriteHelp();
    }

    public void Cancel()
    {
    }

    public CmdHelpWriter HelpWriter { get; private set; }
  }
}