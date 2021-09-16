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
        /// <summary>
        /// Returns all project references, and all project references of all project references, recursively, of the specified project file path.
        /// Does not include the initial project file path in the returned values.
        /// </summary>
        public static async Task<string[]> GetAllRecursiveProjectReferenceDependenciesExclusive(this IVisualStudioProjectFileReferencesProvider visualStudioProjectFileReferencesProvider,
            string projectFilePath)
        {
            var projectFilePathsToProcess = new Queue<string>(EnumerableHelper.From(projectFilePath));
            var projectFilePathsProcessed = new HashSet<string>();

            while(projectFilePathsToProcess.Any())
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

            var output = projectFilePathsProcessed
                .Where(path => path != projectFilePath) // Exclude the initial file path.
                .OrderAlphabetically()
                .ToArray();

            return output;
        }

        /// <summary>
        /// Chooses <see cref="GetAllRecursiveProjectReferenceDependenciesExclusive(IVisualStudioProjectFileReferencesProvider, string)"/> as the default.
        /// </summary>
        public static Task<string[]> GetAllRecursiveProjectReferenceDependencies(this IVisualStudioProjectFileReferencesProvider visualStudioProjectFileReferencesProvider,
           string projectFilePath)
        {
            return visualStudioProjectFileReferencesProvider.GetAllRecursiveProjectReferenceDependenciesExclusive(projectFilePath);
        }
    }
}