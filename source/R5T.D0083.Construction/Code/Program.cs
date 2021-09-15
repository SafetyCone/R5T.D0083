using System;
using System.Threading;
using System.Threading.Tasks;

using Microsoft.Extensions.Hosting;

using R5T.Plymouth;
using R5T.Plymouth.ProgramAsAService;


namespace R5T.D0083.Construction
{
    class Program : ProgramAsAServiceBase
    {
        #region Static
        
        static Task Main()
        {
            return ApplicationBuilder.Instance
                .NewApplication()
                .UseProgramAsAService<Program>()
                .UseT0027_T009_TwoStageStartup<Startup>()
                .BuildProgramAsAServiceHost()
                .Run();
        }

        #endregion
        
        
        private IServiceProvider ServiceProvider { get; }
        
        
        public Program(IApplicationLifetime applicationLifetime,
            IServiceProvider serviceProvider)
            : base(applicationLifetime)
        {
            this.ServiceProvider = serviceProvider;
        }
        
        protected override Task ServiceMain(CancellationToken stoppingToken)
        {
            return this.RunOperation();
        }
        
        private async Task RunOperation()
        {
        
        }
        
        private async Task RunMethod()
        {
        
        }
    }
}