using System;

using Microsoft.Extensions.DependencyInjection;

using R5T.Dacia;
using R5T.Lombardy;


namespace R5T.D0083.I001
{
    public static partial class IServiceCollectionExtensions
    {
        /// <summary>
        /// Adds the <see cref="VisualStudioProjectFileReferencesProvider"/> implementation of <see cref="IVisualStudioProjectFileReferencesProvider"/> as a <see cref="ServiceLifetime.Singleton"/>.
        /// </summary>
        public static IServiceCollection AddVisualStudioProjectFileReferencesProvider_Old(this IServiceCollection services,
            IServiceAction<IStringlyTypedPathOperator> stringlyTypedPathOperatorAction)
        {
            services.AddSingleton<IVisualStudioProjectFileReferencesProvider, VisualStudioProjectFileReferencesProvider>()
                .Run(stringlyTypedPathOperatorAction)
                ;

            return services;
        }

        /// <summary>
        /// Adds the <see cref="VisualStudioProjectFileReferencesProvider"/> implementation of <see cref="IVisualStudioProjectFileReferencesProvider"/> as a <see cref="ServiceLifetime.Singleton"/>.
        /// </summary>
        public static IServiceAction<IVisualStudioProjectFileReferencesProvider> AddVisualStudioProjectFileReferencesProviderAction_Old(this IServiceCollection services,
            IServiceAction<IStringlyTypedPathOperator> stringlyTypedPathOperatorAction)
        {
            var serviceAction = ServiceAction.New<IVisualStudioProjectFileReferencesProvider>(() => services.AddVisualStudioProjectFileReferencesProvider_Old(
                stringlyTypedPathOperatorAction));

            return serviceAction;
        }
    }
}