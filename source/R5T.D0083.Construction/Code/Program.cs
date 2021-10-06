using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

using R5T.Lombardy;
using R5T.Magyar.IO;
using R5T.Plymouth;
using R5T.Plymouth.ProgramAsAService;

using R5T.D0078;


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
            await this.TestGetAllExtraneousProjectsInSolution();
            //await this.TestGetAllRecursiveProjectReferences();
            //await this.TestGetProjectElements();
        }

        private async Task TestGetAllExtraneousProjectsInSolution()
        {
            var solutionFilePath = @"C:\Code\DEV\Git\GitHub\SafetyCone\R5T.D0083\source\R5T.D0083.Construction.sln";

            var solutionFileOperator = this.ServiceProvider.GetRequiredService<IVisualStudioSolutionFileOperator>();

            var projectsInSolutionRelativePaths = await solutionFileOperator.ListProjectReferenceRelativePaths(solutionFilePath);

            var stringlyTypedPathOperator = this.ServiceProvider.GetRequiredService<IStringlyTypedPathOperator>();

            var solutionDirectoryPath = stringlyTypedPathOperator.GetDirectoryPathForFilePath(solutionFilePath);

            var projectsInSolution = projectsInSolutionRelativePaths
                .Select(xRelativePath => stringlyTypedPathOperator.Combine(solutionDirectoryPath, xRelativePath))
                .ToArray();

            var visualStudioProjectFileReferencesProvider = this.ServiceProvider.GetRequiredService<IVisualStudioProjectFileReferencesProvider>();

            var allRecursive = await visualStudioProjectFileReferencesProvider.GetAllRecursiveProjectReferenceDependenciesByProjectFilePath(projectsInSolution);

            var allRecursiveProjectsByProjectTextFilePath = @"C:\Temp\All Recursive Project References by Project.txt";

            allRecursive.WriteToFileInAlphabeticalOrder(allRecursiveProjectsByProjectTextFilePath);

            var extraneousProjectsByProject = await visualStudioProjectFileReferencesProvider.GetExtraneousProjectDependenciesByProjectFilePath(projectsInSolution);

            var extraneousProjectsByProjectOnly = extraneousProjectsByProject
                .Where(xPair => xPair.Value.Length > 1)
                .ToDictionary(
                    xPair => xPair.Key,
                    xPair => xPair.Value);;

            var extraneousProjectsByProjectTextFilePath = @"C:\Temp\Extraneous Project References by Project.txt";

            extraneousProjectsByProjectOnly.WriteToFileInAlphabeticalOrder(extraneousProjectsByProjectTextFilePath);
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