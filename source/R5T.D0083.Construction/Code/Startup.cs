using System;
using System.Threading.Tasks;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

using R5T.Dacia;

using R5T.D0077.Configuration;
using R5T.D0078.A001;
using R5T.T0027.T008;

using R5T.D0083.I001;


namespace R5T.D0083.Construction
{
    class Startup : T0027.T009.Startup
    {
        public Startup(ILogger<Startup> logger)
            : base(logger)
        {
        
        }
        
        public override async Task ConfigureConfiguration(
            IConfigurationBuilder configurationBuilder,
            IServiceProvider startupServicesProvider)
        {
            await configurationBuilder.AddDotnetConfigurationSecretsFilePath(startupServicesProvider);
        }
        
        protected override async Task ConfigureServicesWithProvidedServices(
            IServiceCollection services,
            IServiceAction<IConfiguration> configurationAction,
            IServiceProvider startupServicesProvider,
            IProvidedServices providedServices)
        {
            await base.ConfigureServicesWithProvidedServices(
                services,
                configurationAction,
                startupServicesProvider,
                providedServices);

            // Services.
            var visualStudioSolutionFileOperatorServices = services.AddVisualStudioSolutionFileOperatorServices(
                configurationAction,
                providedServices.FileNameOperatorAction,
                providedServices.StringlyTypedPathOperatorAction);

            var visualStudioProjectFileReferencesProviderAction = services.AddVisualStudioProjectFileReferencesProviderAction(
                providedServices.StringlyTypedPathOperatorAction);

            // Operations.

            // Run.
            services
                // Services.
                .Run(visualStudioProjectFileReferencesProviderAction)
                .Run(visualStudioSolutionFileOperatorServices.VisualStudioSolutionFileOperatorAction)
                ;
        }
    }
}