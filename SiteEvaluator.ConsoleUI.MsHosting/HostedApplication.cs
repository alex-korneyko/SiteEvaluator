using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;

namespace SiteEvaluator.ConsoleUI.MsHosting
{
    public class HostedApplication : 
        Application,
        IHostedService
    {
        public HostedApplication(IConsoleView consoleView)
            : base(consoleView, Environment.GetCommandLineArgs()[1..])
        {
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            await base.StartAsync();
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }
    }
}