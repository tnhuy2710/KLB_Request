using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CoreApi.Extensions
{
    public static class DateTimeExtensions
    {
        public static string ToLocalString(this DateTimeOffset dateTime)
        {
            return dateTime.ToString("'Lúc' HH:mm 'ngày' dd/MM/yyyy");
        }
    }
}
