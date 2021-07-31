using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json.Linq;

namespace JacobCore
{
    public static class JTokenExtensions
    {
        /// <summary>
        /// Checks if the given JToken is an empty string, contains no array values, has no properties, is null or undefined, or has a JProperty value matching these conditions, depending on type.
        /// </summary>
        /// <param name="token">JToken to check. If it is a JProperty, the method will be executed on the name and the value of the JProperty.</param>
        /// <returns></returns>
        public static bool IsNullOrEmpty(this JToken token)
        {
            if (token == null)
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
        /// Checks if the JToken contains the given item. If the token is array, it checks if one of the array items is the passed item. If the token is a string, it will search the string for the string version of the item. If the token is an object, it will search for a property named after the item. If the token is a property, it will run recursively on the property's value.
        /// </summary>
        /// <param name="token">Token that will be searched.</param>
        /// <param name="item">Item to search for.</param>
        /// <returns>True if the token contains the item; false if the token is not a searchable type.</returns>
        public static bool Contains<T>(this JToken token, T item)
        {
            if (token == null)
            {
                return false;
            }
            else
            {
                return token.Type switch
                {
                    JTokenType.String => token.ToString().Contains(item.ToString()),
                    JTokenType.Array => ((JArray)token).ContainsTyped(item),
                    JTokenType.Object => ((JObject)token).ContainsKey(item.ToString()),
                    JTokenType.Property => ((JProperty)token).Value.Contains(item),
                    _ => false
                };
            }
        }

        /// <summary>
        /// Attempts to add passed item to the given JToken. If the JToken is not an array, throws an exception.
        /// </summary>
        /// <param name="token">JToken to add to if it is an array.</param>
        /// <param name="item">Item to add to the array.</param>
        public static void Add<T>(this JToken token, T item)
        {
            if (token == null)
            {
                throw new ArgumentException("Attempted to add item " + item + " to null token.");
            }
            else
            {
                switch(token.Type)
                {
                    case JTokenType.Array:
                        ((JArray)token).Add(item);
                        break;
                    default:
                        throw new ArgumentException("Attempted to add item " + item + " to non-array JToken.");
                }
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
    }
}
