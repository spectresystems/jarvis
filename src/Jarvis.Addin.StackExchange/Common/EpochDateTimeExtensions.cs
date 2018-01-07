using System;

namespace Jarvis.Addin.StackExchange.Common
{
    internal static class EpochDateTimeExtensions
    {
        private static readonly DateTime Epoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);

        public static double ToEpoch(this DateTime date)
        {
            return (date - Epoch).TotalSeconds;
        }
    }
}
