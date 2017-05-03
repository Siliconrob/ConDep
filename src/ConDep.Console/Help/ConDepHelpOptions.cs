namespace ConDep.Console.Help
{
  public class ConDepHelpOptions
  {
    public ConDepCommand Command { get; set; }

    public bool NoOptions()
    {
      return Command == ConDepCommand.NotFound;
    }
  }
}