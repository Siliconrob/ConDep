namespace ConDep.Console
{
  public interface IHandleConDepCommands //<TParser, TValidator>
  {
    CmdHelpWriter HelpWriter { get; }

    void Execute(CmdHelpWriter helpWriter);

    //TParser Parser { get; }
    //TValidator Validator { get; }
    void WriteHelp();

    void Cancel();
  }
}