namespace ConDep.Console.Server
{
  public class CmdServerHandler : IHandleConDepCommands
  {
    private readonly CmdServerHelpWriter _helpWriter;
    private readonly CmdServerParser _parser;

    public CmdServerHandler(string[] args)
    {
      _parser = new CmdServerParser(args);
      _helpWriter = new CmdServerHelpWriter(System.Console.Out);
    }

    public void Execute(CmdHelpWriter helpWriter)
    {
    }

    public void WriteHelp()
    {
      _helpWriter.WriteHelp(_parser.OptionSet);
    }

    public void Cancel()
    {
    }

    public CmdHelpWriter HelpWriter => _helpWriter;
  }
}