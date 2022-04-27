using System;

using R5T.D0083.A001;


namespace System
{
    public static class IServiceAggregationIncrementExtensions
    {
        public static T FillFrom<T>(this T aggregation,
            IServiceAggregationIncrement _)
            where T : IServiceAggregationIncrement
        {
            return aggregation;
        }
    }
}