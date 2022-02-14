using System;
using System.Threading.Tasks;

using R5T.T0064;


namespace R5T.D0083
{
    /// <summary>
    /// Stringly-typed, project reference file paths provider.
    /// </summary>
    [ServiceDefinitionMarker]
    public interface IVisualStudioProjectFileReferencesProvider : IServiceDefinition
    {
        /// <summary>
        /// Given a project file path, get the project files directly referenced by that file path.
        /// </summary>
        Task<string[]> GetProjectReferencesForProject(string projectFilePath);
    }
}