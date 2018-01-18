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
using System;
using System.Collections.Generic;
using System.Linq;
using ArkUtility.Infrastructure.Extensions;

namespace ArkUtility.Infrastructure
{
    public static class Converter
    {
        /// <summary>
        /// Returns a dictionary of the enumeration names and values
        /// </summary>
        /// <typeparam name="TValueType">Value type of enum (short|int|long)</typeparam>
        /// <param name="enumeration"></param>
        /// <example><![CDATA[EnumTypeToDictionary<int>(someValidEnumValue)]]></example>
        /// <returns></returns>
        public static Dictionary<string, TValueType> EnumTypeToDictionary<TValueType>(Enum enumeration) where TValueType : struct
        {
            if (enumeration == null)
                throw new ArgumentNullException(nameof(enumeration));
            return EnumTypeToDictionary<TValueType>(enumeration.GetType());
        }
        /// <summary>
        /// Returns a dictionary of the enumeration names and values
        /// </summary>
        /// <param name="enumerationType"></param>
        /// <returns></returns>
        /// <typeparam name="TValueType">Value type of enum (short|int|long)</typeparam>
        /// <example><![CDATA[EnumTypeToDictionary<int>(someValidEnumType)]]></example>
        /// <exception cref="NotSupportedException"></exception>
        public static Dictionary<string, TValueType> EnumTypeToDictionary<TValueType>(Type enumerationType) where TValueType : struct
        {
            var result = new Dictionary<string, TValueType>();
            if (enumerationType.IsEnum)
            {
                foreach (var name in Enum.GetValues(enumerationType))
                {
                    result.SafeAdd(name.ToString(), (TValueType)name);
                }
            }
            else
            {
                throw new NotSupportedException($"{nameof(EnumTypeToDictionary)}() requires Enumeration Type");
            }
            return result;
        }

    }
}