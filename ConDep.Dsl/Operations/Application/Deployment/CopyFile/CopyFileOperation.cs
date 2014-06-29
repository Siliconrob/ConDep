﻿using System.Threading;
using ConDep.Dsl.Config;
using ConDep.Dsl.Logging;
using ConDep.Dsl.Remote.Node;
using ConDep.Dsl.SemanticModel;

namespace ConDep.Dsl.Operations.Application.Deployment.CopyFile
{
    public class CopyFileOperation : IOperateRemote
    {
        private readonly string _srcFile;
        private readonly string _dstFile;
        private Api _api;

        public CopyFileOperation(string srcFile, string dstFile)
        {
            _srcFile = srcFile;
            _dstFile = dstFile;
        }

        public void Execute(ServerConfig server, IReportStatus status, ConDepSettings settings, CancellationToken token)
        {
            _api = new Api(string.Format("http://{0}/ConDepNode/", server.Name), server.DeploymentUser.UserName, server.DeploymentUser.Password, settings.Options.ApiTimout);
            var result = _api.SyncFile(_srcFile, _dstFile);

            if (result == null) return;

            if (result.Log.Count > 0)
            {
                foreach (var entry in result.Log)
                {
                    Logger.Info(entry);
                }
            }
            else
            {
                Logger.Info("Nothing to deploy. Everything is in sync.");
            }
        }

        public string Name { get { return "Copy File"; } }

        public bool IsValid(Notification notification)
        {
            return true;
        }
    }
}