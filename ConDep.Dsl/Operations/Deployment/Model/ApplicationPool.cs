using System;

namespace ConDep.Dsl
{
    public class ApplicationPool
    {
        public string Name { get; set; }

        public NetFrameworkVersion? NetFrameworkVersion { get; set; }

        public ManagedPipeline? ManagedPipeline { get; set; }

        public string IdentityUsername { get; set; }

        public string IdentityPassword { get; set; }

        public bool? Enable32Bit { get; set; }

        public int? IdleTimeoutInMinutes { get; set; }

        public bool? LoadUserProfile { get; set; }

        public int? RecycleTimeInMinutes { get; set; }
    }
}