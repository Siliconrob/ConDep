﻿using ConDep.Dsl.Core;

namespace ConDep.Dsl
{
    public class DeploymentExistingIisOptions : IProvideForDeploymentExistingIis
    {
        private readonly ISetupWebDeploy _webDeploySetup;

        public DeploymentExistingIisOptions(ISetupWebDeploy webDeploySetup)
        {
            _webDeploySetup = webDeploySetup;
        }

        public ISetupWebDeploy WebDeploySetup1
        {
            get { return _webDeploySetup; }
        }
    }
}