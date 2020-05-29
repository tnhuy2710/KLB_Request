using System;
using System.Data;

namespace CoreApi.Extensions
{
    public static class DataRowExtensions
    {
        public static string TryGetStringValue(this DataRow row, string columnName, string defaultValue = "")
        {
            // Check column exist
            if (row.Table.Columns.IndexOf(columnName) >= 0)
                return row[columnName].ToString().Trim();

            return defaultValue;
        }

        public static string TryGetStringValue(this DataRow row, int columnIndex, string defaultValue = "")
        {
            // Check column exist
            if (row.Table.Columns.Count >= columnIndex + 1)
                return row[columnIndex].ToString().Trim();

            return defaultValue;
        }

        public static int TryGetIntValue(this DataRow row, string columnName, int defaultValue = 0)
        {
            // Check column exist
            var value = row.TryGetStringValue(columnName);
            if (!string.IsNullOrEmpty(value))
                if (int.TryParse(value, out var i))
                    return i;

            return defaultValue;
        }

        public static int TryGetIntValue(this DataRow row, int columnIndex, int defaultValue = 0)
        {
            // Check column exist
            var value = row.TryGetStringValue(columnIndex);
            if (!string.IsNullOrEmpty(value))
                if (int.TryParse(value, out var i))
                    return i;

            return defaultValue;
        }

        public static long TryGetLongValue(this DataRow row, string columnName, long defaultValue = 0)
        {
            // Check column exist
            var value = row.TryGetStringValue(columnName);
            if (!string.IsNullOrEmpty(value))
                if (long.TryParse(value, out var i))
                    return i;

            return defaultValue;
        }

        public static long TryGetLongValue(this DataRow row, int columnIndex, long defaultValue = 0)
        {
            // Check column exist
            var value = row.TryGetStringValue(columnIndex);
            if (!string.IsNullOrEmpty(value))
                if (long.TryParse(value, out var i))
                    return i;

            return defaultValue;
        }

        public static double TryGetDoubleValue(this DataRow row, string columnName, double defaultValue = 0)
        {
            // Check column exist
            var value = row.TryGetStringValue(columnName);
            if (!string.IsNullOrEmpty(value))
                if (double.TryParse(value, out var i))
                    return i;

            return defaultValue;
        }

        public static double TryGetDoubleValue(this DataRow row, int columnIndex, double defaultValue = 0)
        {
            // Check column exist
            var value = row.TryGetStringValue(columnIndex);
            if (!string.IsNullOrEmpty(value))
                if (double.TryParse(value, out var i))
                    return i;

            return defaultValue;
        }

        public static float TryGetFloatValue(this DataRow row, string columnName, float defaultValue = 0)
        {
            // Check column exist
            var value = row.TryGetStringValue(columnName);
            if (!string.IsNullOrEmpty(value))
                if (float.TryParse(value, out var i))
                    return i;

            return defaultValue;
        }

        public static float TryGetFloatValue(this DataRow row, int columnIndex, float defaultValue = 0)
        {
            // Check column exist
            var value = row.TryGetStringValue(columnIndex);
            if (!string.IsNullOrEmpty(value))
                if (float.TryParse(value, out var i))
                    return i;

            return defaultValue;
        }

        public static bool TryGetBooleanValue(this DataRow row, string columnName, bool defaultValue = false)
        {
            // Check column exist
            var value = row.TryGetStringValue(columnName);
            if (!string.IsNullOrEmpty(value))
                if (bool.TryParse(value, out var i))
                    return i;

            return defaultValue;
        }

        public static bool TryGetBooleanValue(this DataRow row, int columnIndex, bool defaultValue = false)
        {
            // Check column exist
            var value = row.TryGetStringValue(columnIndex);
            if (!string.IsNullOrEmpty(value))
                if (bool.TryParse(value, out var i))
                    return i;

            return defaultValue;
        }

        public static DateTime TryGetDateTimeValue(this DataRow row, string columnName, DateTime defaultValue = default(DateTime))
        {
            // Check column exist
            var value = row.TryGetStringValue(columnName);
            if (!string.IsNullOrEmpty(value))
                if (DateTime.TryParse(value, out var i))
                    return i;

            return defaultValue;
        }

        public static DateTime TryGetDateTimeValue(this DataRow row, int columnIndex, DateTime defaultValue = default(DateTime))
        {
            // Check column exist
            var value = row.TryGetStringValue(columnIndex);
            if (!string.IsNullOrEmpty(value))
                if (DateTime.TryParse(value, out var i))
                    return i;

            return defaultValue;
        }
    }
}
