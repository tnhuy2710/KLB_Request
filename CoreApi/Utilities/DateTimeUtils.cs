using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CoreApi.Utilities
{
    public class DateTimeUtils
    {
        public static bool IsValid(DateTimeOffset startDate, DateTimeOffset endDate)
        {
            return (startDate <= DateTimeOffset.UtcNow && DateTimeOffset.UtcNow <= endDate);
        }

        public static bool IsValid(DateTimeOffset? startDate, DateTimeOffset? endDate)
        {
            if (startDate.HasValue && endDate.HasValue)
                return (startDate <= DateTimeOffset.UtcNow && DateTimeOffset.UtcNow <= endDate);
            return false;
        }
    }
}
