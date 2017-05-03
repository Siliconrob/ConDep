using System;
using System.Globalization;
using System.Management.Automation.Host;
using System.Reflection;
using System.Threading;

namespace ConDep.Dsl.Remote
{
  internal class ConDepPSHost : PSHost
  {
    private readonly ConDepPSHostUI _hostUi;

    public ConDepPSHost()
    {
      InstanceId = Guid.NewGuid();
      _hostUi = new ConDepPSHostUI();
    }

    public override string Name => "ConDepPSHost";

    public override Version Version => Assembly.GetExecutingAssembly().GetName().Version;

    public override Guid InstanceId { get; }

    public override PSHostUserInterface UI => _hostUi;

    public override CultureInfo CurrentCulture => Thread.CurrentThread.CurrentCulture;

    public override CultureInfo CurrentUICulture => Thread.CurrentThread.CurrentUICulture;

    public override void SetShouldExit(int exitCode)
    {
      throw new NotImplementedException();
      //this.program.ShouldExit = true;
      //this.program.ExitCode = exitCode;
    }

    public override void EnterNestedPrompt()
    {
      //throw new NotImplementedException();
    }

    public override void ExitNestedPrompt()
    {
      //throw new NotImplementedException();
    }

    public override void NotifyBeginApplication()
    {
    }

    public override void NotifyEndApplication()
    {
    }
  }
}