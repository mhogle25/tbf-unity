using System;
using System.Collections.Generic;
using UnityEngine;

namespace BF2D
{
    public class DEBUGGER
    {
        private static int[] depths = new int[1025];
        public static void set_depths(List<List<int>> indexes, int depthIndex, int depth)
        {
            depths[depthIndex] = depth;
            if (indexes[depthIndex][0] > 0) set_depths(indexes, indexes[depthIndex][0], depth + 1);
            if (indexes[depthIndex][1] > 0) set_depths(indexes, indexes[depthIndex][1], depth + 1);
        }

        public static List<List<int>> swapNodes(List<List<int>> indexes, List<int> queries)
        {
            indexes.Insert(0, new List<int>());
            set_depths(indexes, 1, 1);

            foreach (int query in queries)
            {
                for (int i = 1; i < indexes.Count; i++)
                {
                    if (depths[i] % query == 0)
                    {
                        (indexes[i][1], indexes[i][0]) = (indexes[i][0], indexes[i][1]);
                    }
                }
            }

            indexes.RemoveAt(0);
            return indexes;
        }
        public static int MaxArea(int[] height)
        {
            int leftIndex = 0;
            int rightIndex = height.Length - 1;
            int result = 0;

            while (leftIndex < rightIndex)
            {
                int area = Math.Min(height[leftIndex], height[rightIndex]) * (rightIndex - leftIndex);
                result = Math.Max(area, result);

                if (height[leftIndex] < height[rightIndex])
                {
                    leftIndex++;
                }
                else
                {
                    rightIndex--;
                }
            }

            return result;
        }

        public static int MyAtoi(string s)
        {
            if (s.Length < 1)
            {
                return 0;
            }

            Dictionary<char, int> characterMap = new()
            {
                { '0', 0 },
                { '1', 1 },
                { '2', 2 },
                { '3', 3 },
                { '4', 4 },
                { '5', 5 },
                { '6', 6 },
                { '7', 7 },
                { '8', 8 },
                { '9', 9 }
            };

            int parseIndex = 0;
            Debug.Log($"ParseIndex: {parseIndex}");

            for (; parseIndex < s.Length; parseIndex++)
            {
                if (characterMap.ContainsKey(s[parseIndex]) || s[parseIndex] == '-' || s[parseIndex] == '+')
                {
                    break;
                }

                if (s[parseIndex] != ' ')
                {
                    return 0;
                }
            }
            Debug.Log($"ParseIndex: {parseIndex}");

            int sign = 1;
            if (s.Length > 1)
            {
                if (s[parseIndex] == '-')
                {
                    sign = -1;
                    parseIndex++;
                }

                if (s[parseIndex] == '+')
                {
                    parseIndex++;
                }
            }

            int startIndex = parseIndex;
            int endIndex = s.Length - 1;

            for (; parseIndex < s.Length; parseIndex++)
            {
                if (!characterMap.ContainsKey(s[parseIndex]))
                {
                    endIndex = parseIndex - 1;
                    break;
                }
            }

            Debug.Log($"StartIndex: {startIndex}");
            Debug.Log($"EndIndex: {endIndex}");
            int result = 0;
            for (int digitIndex = endIndex, powerIndex = 0; digitIndex >= startIndex && powerIndex < 10; digitIndex--, powerIndex++)
            {
                int digit = characterMap[s[digitIndex]];
                if (powerIndex == 9)
                {
                    if (digit > 2)
                    {
                        result = int.MaxValue;
                        break;
                    }
                }

                int calculatedAddition = digit * Power(10, powerIndex);
                if (int.MaxValue - result < calculatedAddition)
                {
                    result = int.MaxValue;
                    break;
                }
                result += calculatedAddition;
            }

            result *= sign;

            return result;
        }

        public static int Power(int value, int exponent)
        {
            int result = 1;
            for (int i = 0; i < exponent; i++)
            {
                result *= value;
            }
            return result;
        }

        /// <summary>
        /// string testString = "au";
        /// Debug.Log($"SOLUTION FOR {testString}: {BF2D.DEBUGGER.LengthOfLongestSubstring(testString)}");
        /// </summary>
        /// <param iconID="s"></param>
        /// <returns></returns>
        ///
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
