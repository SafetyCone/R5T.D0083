using System;
using System.Linq;
using System.Xml.Linq;
using System.Xml.XPath;
using System.Threading.Tasks;

using R5T.Dacia;
using R5T.Lombardy;


namespace R5T.D0083.I001
{
    /// <summary>
    /// An implementation based on directly reading Visual Studio project information as XML.
    /// This is much faster than running the dotnet tool and processing its output.
    /// </summary>
    [ServiceImplementationMarker]
    public class VisualStudioProjectFileReferencesProvider : IVisualStudioProjectFileReferencesProvider
    {
        private IStringlyTypedPathOperator StringlyTypedPathOperator { get; }


        public VisualStudioProjectFileReferencesProvider(
            IStringlyTypedPathOperator stringlyTypedPathOperator)
        {
            this.StringlyTypedPathOperator = stringlyTypedPathOperator;
        }

        public Task<string[]> GetProjectReferencesForProject(string projectFilePath)
        {
            var projectReferenceXDocumentRelativeXPath = "//Project/ItemGroup/ProjectReference";
            var projectReferenceIncludeAttributeName = "Include";

            var projectDirectoryPath = this.StringlyTypedPathOperator.GetDirectoryPathForFilePath(projectFilePath);

            var projectXDocument = XDocument.Load(projectFilePath);

            var projectReferenceXElements = projectXDocument.XPathSelectElements(projectReferenceXDocumentRelativeXPath);

            var projectReferenceProjectDirectoryRelativeFilePaths = projectReferenceXElements
                .Select(xElement => xElement.Attribute(projectReferenceIncludeAttributeName).Value)
                .ToArray();

            var output = projectReferenceProjectDirectoryRelativeFilePaths
                .Select(filePath => this.StringlyTypedPathOperator.Combine(
                    projectDirectoryPath,
                    filePath))
                .ToArray();

            return Task.FromResult(output);
        }
    }
}