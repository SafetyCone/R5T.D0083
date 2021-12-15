using System;

using Microsoft.Extensions.DependencyInjection;

using R5T.Lombardy;

using R5T.T0063;


namespace R5T.D0083.I001
{
    public static partial class IServiceCollectionExtensions
    {
        /// <summary>
        /// Adds the <see cref="VisualStudioProjectFileReferencesProvider"/> implementation of <see cref="IVisualStudioProjectFileReferencesProvider"/> as a <see cref="ServiceLifetime.Singleton"/>.
        /// </summary>
        public static IServiceCollection AddVisualStudioProjectFileReferencesProvider(this IServiceCollection services,
            IServiceAction<IStringlyTypedPathOperator> stringlyTypedPathOperatorAction)
        {
            services.AddSingleton<IVisualStudioProjectFileReferencesProvider, VisualStudioProjectFileReferencesProvider>()
                .Run(stringlyTypedPathOperatorAction)
                ;

            return services;
        }
    }
}