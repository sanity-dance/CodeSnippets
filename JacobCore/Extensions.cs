using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json.Linq;

namespace JacobCore
{
    public static class Extensions
    {
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
                    JTokenType.Property => ((JProperty)token).Value.IsNullOrEmpty(),
                    _ => false
                };
            }
        }

        public static bool ContainsTyped<T>(this JArray arr, T item)
        {
            return arr.Any(item =>
                {
                    T typed;
                    try
                    {
                        typed = item.ToObject<T>();
                        return typed.Equals(item);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine("Couldn't parse array item {0} as type {1}: {2}", item, typeof(T), ex);
                        return false;
                    }
                });
        }

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
