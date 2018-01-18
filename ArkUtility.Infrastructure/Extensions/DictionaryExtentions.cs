/*
 * (Licence Notice (LGPLv3))

This file is part of ArkUtility Infrastructure.

ArkUtility Infrastructure is free software: you can redistribute it and / or modify
it under the terms of the GNU Lesser General Public License as published by
the Free Software Foundation, either version 3 of the License, or
(at your option) any later version.

ArkUtility Infrastructure is distributed in the hope that it will be useful,
but WITHOUT ANY WARRANTY; without even the implied warranty of
MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.See the
GNU Lesser General Public License for more details.

You should have received a copy of the GNU Lesser General Public License
along with ArkUtility Infrastructure.If not, see < http://www.gnu.org/licenses/ >.
 */
using System.Collections.Generic;

namespace ArkUtility.Infrastructure.Extensions
{
    public static class DictionaryExtentions
    {
        /// <summary>
        /// Safely adds the key/value or overwrites value if it already exists.
        /// </summary>
        /// <typeparam name="TKey"></typeparam>
        /// <typeparam name="TValue"></typeparam>
        /// <param name="dictionary"></param>
        /// <param name="key">The Key value to add/set</param>
        /// <param name="value">The Value to set the key to.</param>
        public static void SafeAdd<TKey, TValue>(this IDictionary<TKey, TValue> dictionary, TKey key, TValue value)
        {
            dictionary[key] = value;
        }

        /// <summary>
        /// Performs a safe read operation on a dictionary object. If the key is not found then returns the default value.
        /// </summary>
        /// <typeparam name="TKey"></typeparam>
        /// <typeparam name="TValue"></typeparam>
        /// <param name="dictionary"></param>
        /// <param name="key">The key value you wish to retrieve</param>
        /// <param name="defaultValue">Value to return if key is not found</param>
        /// <returns></returns>
        public static TValue SafeRead<TKey, TValue>(this IDictionary<TKey, TValue> dictionary, TKey key, TValue defaultValue)
        {
            //If dictionary contains key return value.
            if (key != null && dictionary.ContainsKey(key))
            {
                return dictionary[key];
            }
            //If key was not found get default value.
            return defaultValue;
        }
    }
}