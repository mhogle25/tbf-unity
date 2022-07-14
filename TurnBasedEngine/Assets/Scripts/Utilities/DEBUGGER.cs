using System;
using System.Collections.Generic;
using UnityEngine;

namespace BF2D
{
    public class DEBUGGER
    {
        /// <summary>
        /// string testString = "au";
        /// Debug.Log($"SOLUTION FOR {testString}: {BF2D.DEBUGGER.LengthOfLongestSubstring(testString)}");
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        public static int LengthOfLongestSubstring(string s)
        {
            
            if (s.Length < 1)
                return 0;

            if (s.Length < 2)
                return 1;

            int i = 0;
            int j = 1;
            HashSet<char> chars = new()
            {
                s[i]
            };

            while (true)
            {
                if (i >= s.Length || j >= s.Length)
                    break;

                if (chars.Contains(s[j]))
                {
                    Debug.Log($"Found duplicate. i = {i} j = {j}");
                    i++;
                    chars.Clear();

                    for (int k = i; k <= j; k++)
                    {
                        if (chars.Contains(s[k]))
                        {
                            chars.Clear();
                            i++;
                            j++;
                            k = i;

                            if (i >= s.Length || j >= s.Length)
                                break;
                        }
                        chars.Add(s[k]);
                    }

                    j++;
                }
                else
                {
                    Debug.Log($"No duplicate, continuing. i = {i} j = {j}");
                    chars.Add(s[j]);
                    j++;
                }
            }

            return Math.Abs(i - j);
        }
    }
}
