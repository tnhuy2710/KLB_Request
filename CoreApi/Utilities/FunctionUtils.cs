using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CoreApi.Extensions;

namespace CoreApi.Utilities
{
    public static class FunctionUtils
    {
        /// <summary>
        /// Trying to get function from string.
        /// </summary>
        /// <param name="content">String content</param>
        /// <param name="functionName">Function name want to get</param>
        /// <param name="function">Result of full function</param>
        /// <param name="functionContent">Result of content function</param>
        /// <returns></returns>
        public static bool TryGetFunction(this string content, string functionName, out string function, out string functionContent)
        {
            if (content.MakeLowerCase().Contains($"{functionName.MakeLowerCase()}("))
            {
                var startIndex = content.IndexOf($"{functionName}(", StringComparison.OrdinalIgnoreCase);
                var charArrays = content.ToCharArray();

                int countOfOpenParenthesis = 0;
                int countOfCloseParenthesis = 0;

                for (int i = startIndex; i < charArrays.Length; i++)
                {
                    var charac = charArrays[i];

                    // Increment open parenthesis
                    if (charac == '(') countOfOpenParenthesis++;

                    // Detect end of function
                    if (charac == ')')
                    {
                        countOfCloseParenthesis++;

                        if (countOfOpenParenthesis.Equals(countOfCloseParenthesis))
                        {
                            // Get index of end function
                            function = content.Substring(startIndex, i - startIndex + 1);
                            functionContent = content.Substring(startIndex + functionName.Length + 1, i - startIndex - functionName.Length - 1);

                            return true;
                        }
                    }
                }
            }

            function = null;
            functionContent = null;
            return false;
        }

    }
}
