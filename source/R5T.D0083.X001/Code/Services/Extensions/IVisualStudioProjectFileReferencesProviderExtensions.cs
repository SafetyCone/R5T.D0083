using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using R5T.Magyar;

using R5T.D0083;


namespace System
{
    public static class IVisualStudioProjectFileReferencesProviderExtensions
    {
        public static async Task<string[]> GetAllRecursiveProjectReferenceDependencies(this IVisualStudioProjectFileReferencesProvider visualStudioProjectFileReferencesProvider,
            string projectFilePath,
            bool includeInitialProject)
        {
            var projectFilePathsToProcess = new Queue<string>(EnumerableHelper.From(projectFilePath));
            var projectFilePathsProcessed = new HashSet<string>();

            while (projectFilePathsToProcess.Any())
            {
                var projectFilePathToProcess = projectFilePathsToProcess.Dequeue();

                var projectReferenceFilePaths = await visualStudioProjectFileReferencesProvider.GetProjectReferencesForProject(projectFilePathToProcess);

                foreach (var projectReferenceFilePath in projectReferenceFilePaths)
                {
                    if (!projectFilePathsProcessed.Contains(projectReferenceFilePath))
                    {
                        projectFilePathsToProcess.Enqueue(projectReferenceFilePath);
                    }
                }

                projectFilePathsProcessed.Add(projectFilePathToProcess);
            }

            if(!includeInitialProject)
            {
                projectFilePathsProcessed.Remove(projectFilePath);
            }

            var output = projectFilePathsProcessed
                .OrderAlphabetically()
                .ToArray();

            return output;
        }

        /// <summary>
        /// Returns all project references, and all project references of all project references, recursively, of the specified project file path.
        /// Does not include the initial project file path in the returned values.
        /// </summary>
        public static async Task<string[]> GetAllRecursiveProjectReferenceDependencies_Exclusive(this IVisualStudioProjectFileReferencesProvider visualStudioProjectFileReferencesProvider,
            string projectFilePath)
        {
            var output = await visualStudioProjectFileReferencesProvider.GetAllRecursiveProjectReferenceDependencies(
                projectFilePath,
                false);

            return output;
        }

        /// <summary>
        /// Returns all project references, and all project references of all project references, recursively, of the specified project file path.
        /// Includes the initial project file path in the returned values.
        /// </summary>
        public static async Task<string[]> GetAllRecursiveProjectReferenceDependencies_Inclusive(this IVisualStudioProjectFileReferencesProvider visualStudioProjectFileReferencesProvider,
            string projectFilePath)
        {
            var output = await visualStudioProjectFileReferencesProvider.GetAllRecursiveProjectReferenceDependencies(
                projectFilePath,
                true);

            return output;
        }

        /// <summary>
        /// Chooses <see cref="GetAllRecursiveProjectReferenceDependencies_Exclusive(IVisualStudioProjectFileReferencesProvider, string)"/> as the default.
        /// </summary>
        public static Task<string[]> GetAllRecursiveProjectReferenceDependencies(this IVisualStudioProjectFileReferencesProvider visualStudioProjectFileReferencesProvider,
           string projectFilePath)
        {
            return visualStudioProjectFileReferencesProvider.GetAllRecursiveProjectReferenceDependencies_Exclusive(projectFilePath);
        }

        /// <summary>
        /// Provides a list of direct project dependencies of a project that are dependencies one of the project's other project dependencies (and thus are direct dependencies that can be satified as indirect dependencies).
        /// </summary>
        public static async Task<string[]> GetExtraneousProjectDependencies(this IVisualStudioProjectFileReferencesProvider visualStudioProjectFileReferencesProvider,
            string projectFilePath)
        {
            var directProjectDependencies = await visualStudioProjectFileReferencesProvider.GetProjectReferencesForProject(projectFilePath);

            var dependenciesOfDependencies = new HashSet<string>();
            foreach (var projectDependency in directProjectDependencies)
            {
                var dependencyDependencies = await visualStudioProjectFileReferencesProvider.GetAllRecursiveProjectReferenceDependencies(projectDependency);

                dependenciesOfDependencies.AddRange(dependencyDependencies);
            }

            var extraneousProjectDependencies = directProjectDependencies.Intersect(dependenciesOfDependencies).ToArray();
            return extraneousProjectDependencies;
        }

        public static string[] GetAllRecursiveProjectReferenceDependencies(this IVisualStudioProjectFileReferencesProvider _,
            string projectFilePath,
            Dictionary<string, string[]> directProjectDependenciesByProjectFilePath,
            bool includeInitialProject = false)
        {
            if(!directProjectDependenciesByProjectFilePath.ContainsKey(projectFilePath))
            {
                throw new Exception($"Direct dependencies not found for project file:\n{projectFilePath}");
            }

            var directProjectDependencies = directProjectDependenciesByProjectFilePath[projectFilePath];

            var allRecursiveProjectReferenceDependencies = new HashSet<string>(directProjectDependencies); // Include the direct dependencies.
            foreach (var directProjectDependency in directProjectDependencies)
            {
                var dependencyDependencies = _.GetAllRecursiveProjectReferenceDependencies(
                    directProjectDependency,
                    directProjectDependenciesByProjectFilePath,
                    includeInitialProject);

                allRecursiveProjectReferenceDependencies.AddRange(dependencyDependencies);
            }

            if(includeInitialProject)
            {
                allRecursiveProjectReferenceDependencies.Add(projectFilePath);
            }

            var output = allRecursiveProjectReferenceDependencies.ToArray();
            return output;
        }

        public static
            IEnumerable<(string ProjectFilePath, string[] RecursiveProjectReferenceDependencies)>
        GetAllRecursiveProjectReferenceDependencies(this IVisualStudioProjectFileReferencesProvider _,
            IEnumerable<string> projectFilePaths,
            Dictionary<string, string[]> directProjectDependenciesByProjectFilePath,
            bool includeInitialProject = false)
        {
            var output = projectFilePaths
                .Select(projectFilePath =>
                {
                    var recursiveProjectReferenceDependencies = _.GetAllRecursiveProjectReferenceDependencies(
                        projectFilePath,
                        directProjectDependenciesByProjectFilePath,
                        includeInitialProject);

                    return (projectFilePath, recursiveProjectReferenceDependencies);
                });

            return output;
        }

        public static Dictionary<string, string[]> GetAllRecursiveProjectReferenceDependenciesByProjectFilePath(this IVisualStudioProjectFileReferencesProvider _,
            IEnumerable<string> projectFilePaths,
            Dictionary<string, string[]> directProjectDependenciesByProjectFilePath,
            bool includeInitialProject = false)
        {
            var tuples = _.GetAllRecursiveProjectReferenceDependencies(
                projectFilePaths,
                directProjectDependenciesByProjectFilePath,
                includeInitialProject);

            var output = tuples.ToDictionary(
                xTuple => xTuple.ProjectFilePath,
                xTuple => xTuple.RecursiveProjectReferenceDependencies);

            return output;
        }

        public static async
            Task<IEnumerable<(string ProjectFilePath, string[] RecursiveProjectReferenceDependencies)>>
        GetAllRecursiveProjectReferenceDependencies(this IVisualStudioProjectFileReferencesProvider visualStudioProjectFileReferencesProvider,
            IEnumerable<string> projectFilePaths,
            bool includeInitialProject = false)
        {
            var directProjectDependenciesByProjectFilePath = await visualStudioProjectFileReferencesProvider.GetProjectReferenceDependencies(
                projectFilePaths);

            var output = visualStudioProjectFileReferencesProvider.GetAllRecursiveProjectReferenceDependencies(
                projectFilePaths,
                directProjectDependenciesByProjectFilePath,
                includeInitialProject);

            return output;
        }

        public static async Task<Dictionary<string, string[]>> GetProjectReferenceDependencies(this IVisualStudioProjectFileReferencesProvider visualStudioProjectFileReferencesProvider,
            IEnumerable<string> projectFilePaths)
        {
            // Multi-process for performance.
            var gettingDirectProjectDependenciesByProjectFilePath = new Dictionary<string, Task<string[]>>();
            foreach (var projectFilePath in projectFilePaths)
            {
                var gettingDirectProjectDependencies = visualStudioProjectFileReferencesProvider.GetProjectReferencesForProject(projectFilePath);

                gettingDirectProjectDependenciesByProjectFilePath.Add(projectFilePath, gettingDirectProjectDependencies);
            }

            await Task.WhenAll(gettingDirectProjectDependenciesByProjectFilePath.Values);

            var directProjectDependenciesByProjectFilePath = new Dictionary<string, string[]>();
            foreach (var pair in gettingDirectProjectDependenciesByProjectFilePath)
            {
                var result = await pair.Value;

                directProjectDependenciesByProjectFilePath.Add(pair.Key, result);
            }

            return directProjectDependenciesByProjectFilePath;
        }

        public static async Task<Dictionary<string, string[]>> GetAllRecursiveProjectReferenceDependenciesByProjectFilePath(this IVisualStudioProjectFileReferencesProvider visualStudioProjectFileReferencesProvider,
            IEnumerable<string> projectFilePaths,
            bool includeInitialProject = false)
        {
            var tuples = await visualStudioProjectFileReferencesProvider.GetAllRecursiveProjectReferenceDependencies(
                projectFilePaths,
                includeInitialProject);

            var output = tuples.ToDictionary(
                xTuple => xTuple.ProjectFilePath,
                xTuple => xTuple.RecursiveProjectReferenceDependencies);

            return output;
        }

        public static async Task<string[]> GetAllRecursiveProjectReferenceDependencies_Aggregated(this IVisualStudioProjectFileReferencesProvider visualStudioProjectFileReferencesProvider,
            IEnumerable<string> projectFilePaths,
            bool includeInitialProject = false)
        {
            var tuples = await visualStudioProjectFileReferencesProvider.GetAllRecursiveProjectReferenceDependencies(
                projectFilePaths,
                includeInitialProject);

            var output = tuples
                .SelectMany(xTuple => xTuple.RecursiveProjectReferenceDependencies)
                .Distinct()
                .Now();

            return output;
        }

        public static async Task<(string ProjectFilePath, string[] ExtraneousProjectDependencies)[]> GetExtraneousProjectDependencies(this IVisualStudioProjectFileReferencesProvider visualStudioProjectFileReferencesProvider,
            IEnumerable<string> projectFilePaths)
        {
            var directProjectReferenceDependenciesByProjectFilePath = await visualStudioProjectFileReferencesProvider.GetProjectReferenceDependencies(projectFilePaths);

            var recursiveProjectReferenceDependenciesByProjectFilePath = visualStudioProjectFileReferencesProvider.GetAllRecursiveProjectReferenceDependenciesByProjectFilePath(
                projectFilePaths,
                directProjectReferenceDependenciesByProjectFilePath);

            var output = projectFilePaths
                .Select(xProjectFilePath =>
                {
                    var directProjectReferenceDependencies = directProjectReferenceDependenciesByProjectFilePath[xProjectFilePath];

                    var allDependenciesOfDependencies = new HashSet<string>();
                    foreach (var directDependency in directProjectReferenceDependencies)
                    {
                        var recursiveDependenciesOfDependency = recursiveProjectReferenceDependenciesByProjectFilePath[directDependency];

                        allDependenciesOfDependencies.AddRange(recursiveDependenciesOfDependency);
                    }

                    var extraneousDirectProjectReferenceDependencies = directProjectReferenceDependencies.Intersect(allDependenciesOfDependencies).ToArray();

                    return (xProjectFilePath, extraneousDirectProjectReferenceDependencies);
                })
                .ToArray();

            return output;
        }

        public static async Task<Dictionary<string, string[]>> GetExtraneousProjectDependenciesByProjectFilePath(this IVisualStudioProjectFileReferencesProvider visualStudioProjectFileReferencesProvider,
            IEnumerable<string> projectFilePaths)
        {
            var tuples = await visualStudioProjectFileReferencesProvider.GetExtraneousProjectDependencies(projectFilePaths);

            var output = tuples.ToDictionary(
                xTuple => xTuple.ProjectFilePath,
                xTuple => xTuple.ExtraneousProjectDependencies);

            return output;
        }

        /// <summary>
        /// Here only as related functionatliy. Does not depend on the service.
        /// </summary>
        public static string[] GetRequiredButUnavailableProjects(this IVisualStudioProjectFileReferencesProvider _,
            IEnumerable<string> requiredProjectFilePaths,
            IEnumerable<string> availableProjectFilePaths)
        {
            var output = requiredProjectFilePaths.Except(availableProjectFilePaths)
                .ToArray();

            return output;
        }

        public static async Task<bool> HasDirectProjectReference(this IVisualStudioProjectFileReferencesProvider visualStudioProjectFileReferencesProvider,
            string projectFilePath,
            string referenceProjectFilePath)
        {
            var hasProjectReferenceByReferenceProjectFilePath = await visualStudioProjectFileReferencesProvider.HasDirectProjectReferences(
                projectFilePath,
                EnumerableHelper.From(referenceProjectFilePath));

            var output = hasProjectReferenceByReferenceProjectFilePath[referenceProjectFilePath];
            return output;
        }

        public static async Task<Dictionary<string, bool>> HasDirectProjectReferences(this IVisualStudioProjectFileReferencesProvider visualStudioProjectFileReferencesProvider,
            string projectFilePath,
            IEnumerable<string> referenceProjectFilePaths)
        {
            var allProjectReferenceOfProject = await visualStudioProjectFileReferencesProvider.GetProjectReferencesForProject(projectFilePath);

            var allProjectReferenceOfProjectHash = allProjectReferenceOfProject.ToHashSet();

            // TODO: use a paths operation to compare in a directory separator-insensitive way?
            var output = referenceProjectFilePaths
                .Select(x => (ReferenceProjectFilePath: x, HasReference: allProjectReferenceOfProjectHash.Contains(x)))
                .ToDictionary(
                    x => x.ReferenceProjectFilePath,
                    x => x.HasReference);

            return output;
        }

        /// <summary>
        /// Determines whether the project has the reference project among its recursive project references.
        /// </summary>
        public static async Task<bool> HasRecursiveProjectReference(this IVisualStudioProjectFileReferencesProvider visualStudioProjectFileReferencesProvider,
            string projectFilePath,
            string referenceProjectFilePath)
        {
            var hasProjectReferenceByReferenceProjectFilePath = await visualStudioProjectFileReferencesProvider.HasRecursiveProjectReferences(
                projectFilePath,
                EnumerableHelper.From(referenceProjectFilePath));

            var output = hasProjectReferenceByReferenceProjectFilePath[referenceProjectFilePath];
            return output;
        }

        public static async Task<Dictionary<string, bool>> HasRecursiveProjectReferences(this IVisualStudioProjectFileReferencesProvider visualStudioProjectFileReferencesProvider,
            string projectFilePath,
            IEnumerable<string> referenceProjectFilePaths)
        {
            var allRecursiveProjectReferenceOfProject = await visualStudioProjectFileReferencesProvider.GetAllRecursiveProjectReferenceDependencies_Exclusive(projectFilePath);

            var allRecursiveProjectReferenceOfProjectHash = allRecursiveProjectReferenceOfProject.ToHashSet();

            // TODO: use a paths operation to compare in a directory separator-insensitive way?
            var output = referenceProjectFilePaths
                .Select(x => (ReferenceProjectFilePath: x, HasReference: allRecursiveProjectReferenceOfProjectHash.Contains(x)))
                .ToDictionary(
                    x => x.ReferenceProjectFilePath,
                    x => x.HasReference);

            return output;
        }

        /// <summary>
        /// Chooses <see cref="HasDirectProjectReference(IVisualStudioProjectFileReferencesProvider, string, string)"/> as the default.
        /// </summary>
        public static Task<bool> HasProjectReference(this IVisualStudioProjectFileReferencesProvider visualStudioProjectFileReferencesProvider,
            string projectFilePath,
            string referenceProjectFilePath)
        {
            return visualStudioProjectFileReferencesProvider.HasDirectProjectReference(
                projectFilePath,
                referenceProjectFilePath);
        }

        /// <summary>
        /// Chooses <see cref="HasDirectProjectReferences(IVisualStudioProjectFileReferencesProvider, string, IEnumerable{string})"/> as the default.
        /// </summary>
        public static Task<Dictionary<string, bool>> HasProjectReferences(this IVisualStudioProjectFileReferencesProvider visualStudioProjectFileReferencesProvider,
            string projectFilePath,
            IEnumerable<string> referenceProjectFilePaths)
        {
            return visualStudioProjectFileReferencesProvider.HasDirectProjectReferences(
                projectFilePath,
                referenceProjectFilePaths);
        }
    }
}