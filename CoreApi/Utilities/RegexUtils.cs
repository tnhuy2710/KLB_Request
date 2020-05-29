using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace CoreApi.Utilities
{
    public class RegexUtils
    {
        public static bool Match(string pattern, string input, int groupIndex, out string result)
        {
            result = null;

            var regex = new Regex(pattern, RegexOptions.IgnoreCase);
            var match = regex.Match(input);

            if (match.Success)
            {
                if (match.Groups.Count >= groupIndex)
                {
                    result = match.Groups[groupIndex].Value;
                    return true;
                }
            }

            return false;
        }

        public static bool Matchs(string pattern, string input, out IList<GroupCollection> results)
        {
            results = new List<GroupCollection>();

            var regex = new Regex(pattern, RegexOptions.IgnoreCase);
            var matchs = regex.Matches(input);

            foreach (Match match in matchs)
            {
                if (match.Success)
                    results.Add(match.Groups);
            }

            return results?.Count > 0;
        }
    }
}
