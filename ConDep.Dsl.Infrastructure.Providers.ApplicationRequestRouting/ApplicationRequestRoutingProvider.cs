using ConDep.Dsl.LoadBalancer;
using ConDep.Dsl.WebDeploy;

namespace ConDep.Dsl.Infrastructure.Providers.ApplicationRequestRouting
{
    public class ApplicationRequestRoutingProvider : WebDeployCompositeProviderBase 
    {
        private readonly LoadBalanceState _state;
        private readonly string _serverNameToChangeStateOn;

        public ApplicationRequestRoutingProvider(LoadBalanceState state, string serverNameToChangeStateOn)
        {
            _state = state;
            _serverNameToChangeStateOn = serverNameToChangeStateOn;
        }

        public override bool IsValid(Notification notification)
        {
            return true;
        }

        public override void Configure(DeploymentServer arrServer)
        {
            var condition = WebDeployExecuteCondition<ProvideForInfrastructure>.IsFailure(p => p.PowerShell("import-module ApplicationRequestRouting"));

            DeployPsCmdLet(arrServer, condition);
            Execute(_state, arrServer, _serverNameToChangeStateOn);
            RemovePsCmdLet(arrServer, condition);
        }

        private void DeployPsCmdLet(DeploymentServer server, WebDeployExecuteCondition<ProvideForInfrastructure> condition)
        {
            Configure<ProvideForDeployment>(server, p => p.CopyDir(@"C:\GitHub\ConDep\ConDep.PowerShell.ApplicationRequestRouting\bin\Release", @"%temp%\ApplicationRequestRouting"), condition);
        }

        private void Execute(LoadBalanceState state, DeploymentServer server, string serverNameToChangeStateOn)
        {
            Configure<ProvideForInfrastructure>(server, p => p.PowerShell(string.Format(@"import-module $env:temp\ApplicationRequestRouting; Set-WebFarmServerState -State {0} -Name {1} -UseDnsLookup;", state.ToString(), serverNameToChangeStateOn)));
        }

        private void RemovePsCmdLet(DeploymentServer arrServer, WebDeployExecuteCondition<ProvideForInfrastructure> condition)
        {
            Configure<ProvideForInfrastructure>(arrServer, p => p.PowerShell(@"remove-item $env:temp\ApplicationRequestRouting -force -recurse"), condition);
        }
    }
}