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
using System.IO;
using System.Runtime.Serialization.Json;

namespace ArkUtility.Infrastructure.Extensions
{
    public static class ObjectExtensions
    {
        /// <summary>
        /// Serialize an object as JSON via basic conversion.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="item"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public static string SerializedAsJson<T>(this T item)
        {
            string result = null;
            try
            {
                //Note: You do not need to dispose memory streams, so we not using a "using" block.
                MemoryStream ms = new MemoryStream();
                DataContractJsonSerializer ser = new DataContractJsonSerializer(typeof(T));
                ser.WriteObject(ms, item);
                ms.Position = 0;
                StreamReader sr = new StreamReader(ms);
                result = sr.ReadToEnd();
            }
            catch (Exception ex)
            {
                var valueAddedEx = new Exception($"SerializeAsJson encounterd an exception, likely during Serialization [{typeof(T).FullName}].\r\nError:{ex.Message} \r\nSee Inner Exception.", ex);
                throw valueAddedEx;
            }
            return result;
        }
    }
}