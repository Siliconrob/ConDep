﻿using System;
using System.Threading;
using System.Threading.Tasks;
using ConDep.Dsl.Config;
using ConDep.Dsl.Logging;
using ConDep.Dsl.Remote;
using ConDep.Dsl.SemanticModel;
using ConDep.Dsl.SemanticModel.WebDeploy;
using ConDep.WebQ.Client;

namespace ConDep.Console.Deploy
{
    public class CmdDeployHandler : IHandleConDepCommands
    {
        private CmdDeployParser _parser;
        private CmdDeployValidator _validator;
        private CmdDeployHelpWriter _helpWriter;
        private WebQueue _webQ;
        private CancellationTokenSource _tokenSource;

        public CmdDeployHandler(string[] args)
        {
            _parser = new CmdDeployParser(args);
            _validator = new CmdDeployValidator();
            _helpWriter = new CmdDeployHelpWriter(System.Console.Out);
        }

        public void Execute(CmdHelpWriter helpWriter)
        {
            var conDepSettings = new ConDepSettings();

            try
            {
                conDepSettings.Options = GetOptions(_parser, _validator);
                conDepSettings.Config = ConfigHandler.GetEnvConfig(conDepSettings);

                helpWriter.PrintCopyrightMessage();
                _webQ = WaitInQueue(conDepSettings);

                var status = new ConDepStatus();
                _tokenSource = new CancellationTokenSource();
                var token = _tokenSource.Token;

                var task = ConDepConfigurationExecutor.ExecuteFromAssembly(conDepSettings, status, token);

                task.ContinueWith(result =>
                        {
                            if (result.Result.Success)
                            {
                                status.EndTime = DateTime.Now;
                                status.PrintSummary();
                            }
                            else
                            {
                                Environment.Exit(1);
                            }
                        }, TaskContinuationOptions.OnlyOnRanToCompletion);

                task.Wait();

                if (task.IsFaulted)
                {
                    throw task.Exception;
                }
            }
            finally
            {
                if (_webQ != null)
                {
                    Logger.Info("Leaving WebQ");
                    _webQ.LeaveQueue();
                }

                //Environment.Exit(exitCode);
            }

        }

        public void Cancel()
        {
            if (_tokenSource != null)
            {
                Logger.Warn("Cancelling task!");
                _tokenSource.Cancel();
            }

            if (_webQ != null)
            {
                Logger.Info("Leaving queue...");
                _webQ.LeaveQueue();
            }
        }
        public void WriteHelp()
        {
            _helpWriter.WriteHelp(_parser.OptionSet);
        }

        private ConDepOptions GetOptions(CmdBaseParser<ConDepOptions> parser, CmdBaseValidator<ConDepOptions> validator)
        {
            var options = parser.Parse();
            validator.Validate(options);

            var configAssemblyLoader = new ConDepAssemblyHandler(options.AssemblyName);
            options.Assembly = configAssemblyLoader.GetAssembly();

            return options;
        }

        private static WebQueue WaitInQueue(ConDepSettings conDepSettings)
        {
            if (!string.IsNullOrWhiteSpace(conDepSettings.Options.WebQAddress))
            {
                var webQ = new WebQueue(conDepSettings.Options.WebQAddress, conDepSettings.Options.Environment);
                webQ.WebQueuePositionUpdate += (sender, eventArgs) => Logger.Info(eventArgs.Message);
                webQ.WebQueueTimeoutUpdate += (sender, eventArgs) => Logger.Info(eventArgs.Message);

                Logger.WithLogSection("Waiting in Deployment Queue", () =>
                    {
                        webQ.WaitInQueue(TimeSpan.FromMinutes(30));
                        return webQ;
                    });
            }
            return null;
        }
    }
}