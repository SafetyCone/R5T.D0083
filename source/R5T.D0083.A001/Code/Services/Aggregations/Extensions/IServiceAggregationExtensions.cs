using System;

using R5T.D0083.A001;


namespace System
{
    public static class IServiceAggregationExtensions
    {
        public static T FillFrom<T>(this T aggregation,
            IServiceAggregation _)
            where T : IServiceAggregation
        {
            return aggregation;
        }
    }
}