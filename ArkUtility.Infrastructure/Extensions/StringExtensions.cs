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
using System.Text;
using System.Threading.Tasks;

namespace ArkUtility.Infrastructure.Extensions
{
    public static class StringExtensions
    {
        /// <summary>
        /// Truncates a string in the event it is to long to allow it to be safe to insert into the windows event log.
        /// </summary>
        /// <param name="eventLogMessage"></param>
        /// <returns></returns>
        public static string ToSafeEventLogEntry(this string eventLogMessage)
        {
            if (string.IsNullOrWhiteSpace(eventLogMessage) || eventLogMessage.Length <= 31800)
                return eventLogMessage;
            return eventLogMessage.Substring(0, 31800) + " ... Entry truncated";
        }
        /// <summary>
        /// Encode a String as a Base64String
        /// </summary>
        /// <param name="plainText"></param>
        /// <param name="encoding">Defaults to System.Text.Encoding.UTF8 </param>
        /// <returns></returns>
        public static string Base64Encode(this string plainText, System.Text.Encoding encoding = null)
        {
            if (string.IsNullOrEmpty(plainText))
                return string.Empty;
            var enc = encoding ?? Encoding.UTF8;
            var bytes = enc.GetBytes(plainText);
            return System.Convert.ToBase64String(bytes);
        }
        /// <summary>
        /// Decode a Base64 string to PlainText
        /// </summary>
        /// <param name="encodedText"></param>
        /// <param name="encoding">Defaults to System.Text.Encoding.UTF8 </param>
        /// <returns></returns>
        public static string Base64Decode(this string encodedText, System.Text.Encoding encoding = null)
        {
            if (string.IsNullOrEmpty(encodedText))
                throw new ArgumentNullException(nameof(encodedText));
            var encodedBytes = System.Convert.FromBase64String(encodedText);
            var enc = encoding ?? Encoding.UTF8;
            return enc.GetString(encodedBytes);
        }
    }
}
