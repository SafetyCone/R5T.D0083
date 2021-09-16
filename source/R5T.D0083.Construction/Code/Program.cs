using System;
using System.Threading;
using System.Threading.Tasks;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

using R5T.Magyar.IO;
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
            return this.RunMethod();
            //return this.RunOperation();
        }
        
        private async Task RunOperation()
        {
        
        }
        
        private async Task RunMethod()
        {
            await this.TestGetAllRecursiveProjectReferences();
            //await this.TestGetProjectElements();
        }

        private async Task TestGetAllRecursiveProjectReferences()
        {
            var projectFilePath = @"C:\Code\DEV\Git\GitHub\SafetyCone\R5T.D0083\source\R5T.D0083.Construction\R5T.D0083.Construction.csproj";

            var visualStudioProjectFileReferencesProvider = this.ServiceProvider.GetRequiredService<IVisualStudioProjectFileReferencesProvider>();

            var allRecursiveProjectReferences = await visualStudioProjectFileReferencesProvider.GetAllRecursiveProjectReferenceDependencies(
                projectFilePath);

            await FileHelper.WriteAllLines(
                @"C:\Temp\Project references.txt", allRecursiveProjectReferences);
        }

        private async Task TestGetProjectElements()
        {
            var projectFilePath = @"C:\Code\DEV\Git\GitHub\SafetyCone\R5T.D0083\source\R5T.D0083.Construction\R5T.D0083.Construction.csproj";

            var visualStudioProjectFileReferencesProvider = this.ServiceProvider.GetRequiredService<IVisualStudioProjectFileReferencesProvider>();

            var projectReferences = await visualStudioProjectFileReferencesProvider.GetProjectReferencesForProject(projectFilePath);
        }
    }
}