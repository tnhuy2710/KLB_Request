using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace CoreApi.Extensions
{
    public static class StringExtensions
    {
        public static string MakeLowerCase(this string content)
        {
            if (!string.IsNullOrEmpty(content)) return content.Trim().ToLower();
            return string.Empty;
        }

        public static string MakeUpperCase(this string content)
        {
            if (!string.IsNullOrEmpty(content)) return content.Trim().ToUpper();
            return string.Empty;
        }

        public static string RemoveWhitespace(this string content)
        {
            return content.Replace(" ", "");
        }

        public static bool IsEmptyOrNull(params string[] data)
        {
            foreach (var s in data)
                if (string.IsNullOrEmpty(s)) return true;

            return false;
        }

        public static bool IsValidEmail(this string content)
        {
            var match = Regex.Match(content.Trim(),
                @"([\w+-.%]+@[\w-.]+\.[A-Za-z]{2,10})((,[\w+-.%]+@[\w-.]+\.[A-Za-z]{2,10}){1,})?",
                RegexOptions.IgnoreCase);

            return match.Success;
        }

        public static bool IsNumeric(this string content)
        {
            return long.TryParse(content, out _);
        }

        public static bool IsNumeric(this string content, out long value)
        {
            return long.TryParse(content, out value);
        }

        public static bool IsDouble(this string content)
        {
            return double.TryParse(content, out _);
        }

        public static bool IsDouble(this string content, out double value)
        {
            return double.TryParse(content, out value);
        }

        public static double TryParseToDouble(this string content, double defaultValue = 0)
        {
            return double.TryParse(content, out var val) ? val : defaultValue;
        }

        public static long TryParseToLong(this string content, long defaultValue = 0)
        {
            return long.TryParse(content, out var val) ? val : defaultValue;
        }

        public static bool TryParseToBoolean(this string content, bool defaultValue = false)
        {
            return bool.TryParse(content, out var val) ? val : defaultValue;
        }

        public static string[] TrySplit(this string content, string separator)
        {
            if (!content.Contains(separator)) content += separator;
            return content.Split(new[] {separator}, StringSplitOptions.RemoveEmptyEntries);
        }

        public static string RemoveAccent(this string txt)
        {
            byte[] bytes = System.Text.Encoding.GetEncoding("Cyrillic").GetBytes(txt);
            return System.Text.Encoding.ASCII.GetString(bytes);
        }

        private static readonly string[] VietnameseUnicodeStrings = new string[]
        {
            "aAeEoOuUiIdDyY",
            "áàạảãâấầậẩẫăắằặẳẵ",
            "ÁÀẠẢÃÂẤẦẬẨẪĂẮẰẶẲẴ",
            "éèẹẻẽêếềệểễ",
            "ÉÈẸẺẼÊẾỀỆỂỄ",
            "óòọỏõôốồộổỗơớờợởỡ",
            "ÓÒỌỎÕÔỐỒỘỔỖƠỚỜỢỞỠ",
            "úùụủũưứừựửữ",
            "ÚÙỤỦŨƯỨỪỰỬỮ",
            "íìịỉĩ",
            "ÍÌỊỈĨ",
            "đ",
            "Đ",
            "ýỳỵỷỹ",
            "ÝỲỴỶỸ"
        };

        /// <summary>
        /// This support remove vietnamese unicode string.
        /// </summary>
        /// <param name="content"></param>
        /// <returns></returns>
        public static string RemoveVietnameseString(this string content)
        {
            if (string.IsNullOrEmpty(content))
                return null;

            for (int i = 1; i < VietnameseUnicodeStrings.Length; i++)
            {
                for (int j = 0; j < VietnameseUnicodeStrings[i].Length; j++)
                    content = content.Replace(VietnameseUnicodeStrings[i][j], VietnameseUnicodeStrings[0][i - 1]);
            }
            return content;
        }

        /// <summary>
        /// Handle and reformat phone number.
        /// </summary>
        /// <param name="phoneNumber"></param>
        /// <returns></returns>
        public static string FormatPhoneNumer(this string phoneNumber)
        {
            // Remove all special char
            var phoneNumberFormatted = Regex.Replace(phoneNumber.Trim(), @"[^0-9a-zA-Z]+", "");

            // Handle phone number có +84 hoặc 84 ở đầu
            phoneNumberFormatted = Regex.Replace(phoneNumberFormatted, @"^(\+84|84|0)(\d+)", "0$2");

            if (phoneNumberFormatted.StartsWith("0"))
                return phoneNumberFormatted;

            throw new Exception("Wrong phone number formatted: " + phoneNumberFormatted);
        }

        /// <summary>
        /// Hidden middle numbers in phone number to x.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static string HiddenPhoneNumber(this string value)
        {
            if (string.IsNullOrEmpty(value)) return "";

            // Reformatted phone number
            value = value.FormatPhoneNumer();

            if (value.Length.Equals(10))
                return Regex.Replace(value, @"(\d{3})(\d{4})(\d{3})", $"$1xxxx$3");

            if (value.Length.Equals(11))
                return Regex.Replace(value, @"(\d{4})(\d{4})(\d{3})", $"$1xxxx$3");

            return value;
        }

        public static bool IsValidPhoneNumber(this string value)
        {
            if (value.StartsWith("+84") || value.StartsWith("84") || value.StartsWith("0"))
                return true;
            return false;
        }
    }
}
