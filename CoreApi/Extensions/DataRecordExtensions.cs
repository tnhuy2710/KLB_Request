using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace CoreApi.Extensions
{
    public static class DataRecordExtensions
    {
        public static T GetValueOrDefault<T>(this IDataRecord row, string fieldName)
        {
            int ordinal = row.GetOrdinal(fieldName);
            return row.GetValueOrDefault<T>(ordinal, fieldName);
        }

        public static T GetValueOrDefault<T>(this IDataRecord row, int ordinal, string fieldName = "")
        {
            try
            {
                return (T)(row.IsDBNull(ordinal) ? default(T) : row.GetValue(ordinal));
            }
            catch (Exception e)
            {
                Debug.WriteLine($"Error cant get value of field {fieldName} with message: {e.Message}");
                return default(T);
            }
        }
    }
}
