using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json.Linq;

namespace JacobCore
{
    public static class Extensions
    {
        /// <summary>
        /// Gets a list of all the indices of the given string or char.
        /// </summary>
        /// <param name="str">String to search.</param>
        /// <param name="value">String to search for.</param>
        /// <returns>List of all indices of the search value within the target string.</returns>
        public static List<int> AllIndicesOf(this string str, string value)
        {
            if(string.IsNullOrEmpty(value))
            {
                throw new ArgumentException("The search string must not be empty.");
            }
            List<int> indices = new List<int>();
            for(int i=0; i != -1; i += value.Length)
            {
                i = str.IndexOf(value, i);
                if(i!=-1)
                {
                    indices.Add(i);
                }
            }
            return indices;
        }
        /// <summary>
        /// Gets a list of all the indices of the given string or char.
        /// </summary>
        /// <param name="str">String to search.</param>
        /// <param name="value">Char to search for.</param>
        /// <returns>List of all indices of the search value within the target string.</returns>
        public static List<int> AllIndicesOf(this string str, char value)
        {
            List<int> indices = new List<int>();
            for (int i = 0; i != -1; i += 1)
            {
                i = str.IndexOf(value, i);
                if (i != -1)
                {
                    indices.Add(i);
                }
            }
            return indices;
        }

        /// <summary>
        /// Checks if the given JToken is an empty string, contains no array values, has no properties, is null or undefined, or has a JProperty value matching these conditions, depending on type.
        /// </summary>
        /// <param name="token">JToken to check. If it is a JProperty, the method will be executed on the name and the value of the JProperty.</param>
        /// <returns></returns>
        public static bool IsNullOrEmpty(this JToken token)
        {
            if(token == null)
            {
                return true;
            }
            else
            {
                return token.Type switch
                {
                    JTokenType.String => string.IsNullOrWhiteSpace(token.ToString()),
                    JTokenType.Array => ((JArray)token).Count == 0,
                    JTokenType.Object => ((JObject)token).Count == 0,
                    JTokenType.Null => true,
                    JTokenType.Undefined => true,
                    JTokenType.None => string.IsNullOrWhiteSpace(token.ToString()),
                    JTokenType.Property => string.IsNullOrWhiteSpace(((JProperty)token).Name) && ((JProperty)token).Value.IsNullOrEmpty(),
                    _ => false
                };
            }
        }

        /// <summary>
        /// Searches a JArray for an item of a specific type.
        /// </summary>
        /// <typeparam name="T">Type of the item to search for.</typeparam>
        /// <param name="arr">JArray to search.</param>
        /// <param name="item">Item to search for.</param>
        /// <returns>Bool indicating whether or not the array contains the item.</returns>
        public static bool ContainsTyped<T>(this JArray arr, T item)
        {
            return arr.Any(it =>
                {
                    T typed;
                    try
                    {
                        typed = it.ToObject<T>();
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine("Couldn't parse array item {0} as type {1}: {2}", it, typeof(T), ex);
                        return false;
                    }
                    return typed.Equals(item);
                });
        }

        /// <summary>
        /// Counts the instances of a character in a string.
        /// </summary>
        /// <param name="input">Input string to search.</param>
        /// <param name="target">Char to search for.</param>
        /// <returns>Number of times char occurs in input.</returns>
        public static int CountOfChar(this string input, char target)
        {
            char[] charArray = input.ToCharArray();
            int length = charArray.Length;
            int count = 0;
            for(int i = length - 1; i >= 0; i--)
            {
                if(charArray[i] == target)
                {
                    count++;
                }
            }
            return count;
        }
    }
}
