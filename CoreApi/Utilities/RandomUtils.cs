using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Threading.Tasks;

namespace CoreApi.Utilities
{
    public class RandomUtils
    {
        private static readonly Random Random = new Random();

        /// <summary>
        /// Generate random number password.
        /// </summary>
        /// <param name="numberCharacter">Total number character.</param>
        /// <returns></returns>
        public static string GenerateRandomNumber(int numberCharacter)
        {
            return string.Join("", GenerateRandomNumber(numberCharacter, 0, 9));
        }

        private static List<int> GenerateRandomNumber(int count, int min, int max)
        {
            if (max <= min || count < 0 ||
                // max - min > 0 required to avoid overflow
                count > max - min && max - min > 0)
                throw new ArgumentOutOfRangeException("Range " + min + " to " + max +
                                                      " (" + (max - (long)min) + " values), or count " + count +
                                                      " is illegal");

            // generate count random values.
            var candidates = new HashSet<int>();

            // start count values before max, and end at max
            for (var top = max - count; top < max; top++)
                if (!candidates.Add(Random.Next(min, top + 1))) candidates.Add(top);

            // load them in to a list, to sort
            var result = candidates.ToList();

            // shuffle the results because HashSet has messed
            // with the order, and the algorithm does not produce
            // random-ordered results (e.g. max-1 will never be the first value)
            for (var i = result.Count - 1; i > 0; i--)
            {
                var k = Random.Next(i + 1);
                var tmp = result[k];
                result[k] = result[i];
                result[i] = tmp;
            }
            return result;
        }

        
        public static int RandomNumber(int min, int max)
        {
            var passwordBytes = new byte[4];
            using (var rng = RandomNumberGenerator.Create())
                rng.GetBytes(passwordBytes);
            int rand = (int)(min + (max - min) * (BitConverter.ToInt32(passwordBytes, 0) / (double)Int32.MaxValue));
            passwordBytes = null;
            return rand;
        }
    }
}
