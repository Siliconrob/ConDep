using System.Threading;
using ConDep.Dsl;
using ConDep.Dsl.Config;
using ConDep.Dsl.Logging;
using ConDep.Dsl.Remote;

namespace ConDep.Execution
{
  internal class PostRemoteOps : RemoteOperation
  {
    public override string Name => "Post Remote Operation";

    public Result Result { get; set; }

    public override Result Execute(IOfferRemoteOperations remote, ServerConfig server, ConDepSettings settings,
      CancellationToken token)
    {
      token.ThrowIfCancellationRequested();

      Logger.WithLogSection(string.Format("Stopping ConDepNode on server {0}", server.Name), () =>
      {
        var executor = new PowerShellExecutor();
        executor.Execute(server, "Stop-ConDepNode", mod =>
        {
          mod.LoadConDepModule = false;
          mod.LoadConDepNodeModule = true;
        }, logOutput: false);
      });

      return Result.SuccessUnChanged();
    }

    public void DryRun()
    {
      Logger.WithLogSection(Name, () => { });
    }
  }
}