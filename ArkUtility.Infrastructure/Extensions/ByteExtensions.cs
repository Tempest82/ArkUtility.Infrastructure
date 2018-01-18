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
    public static class ByteExtensions
    {
        /// <summary>
        /// Convert a Byte Array to a Base64String
        /// </summary>
        /// <param name="bytes"></param>
        /// <param name="formatOptions"></param>
        /// <returns></returns>
        public static string Base64Encode(this byte[] bytes, Base64FormattingOptions formatOptions =  Base64FormattingOptions.None)
        {
            if (bytes?.Any() != true)
                return string.Empty;
            return System.Convert.ToBase64String(bytes, formatOptions);
        }

    }
}
