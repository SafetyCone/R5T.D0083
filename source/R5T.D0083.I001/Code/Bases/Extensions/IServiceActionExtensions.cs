using System;

using R5T.Lombardy;

using R5T.T0062;
using R5T.T0063;


namespace R5T.D0083.I001
{
    public static class IServiceActionExtensions
    {
        /// <summary>
        /// Adds the <see cref="VisualStudioProjectFileReferencesProvider"/> implementation of <see cref="IVisualStudioProjectFileReferencesProvider"/> as a <see cref="Microsoft.Extensions.DependencyInjection.ServiceLifetime.Singleton"/>.
        /// </summary>
        public static IServiceAction<IVisualStudioProjectFileReferencesProvider> AddVisualStudioProjectFileReferencesProviderAction(this IServiceAction _,
            IServiceAction<IStringlyTypedPathOperator> stringlyTypedPathOperatorAction)
        {
            var serviceAction = _.New<IVisualStudioProjectFileReferencesProvider>(services => services.AddVisualStudioProjectFileReferencesProvider(
                stringlyTypedPathOperatorAction));

            return serviceAction;
        }
    }
}
