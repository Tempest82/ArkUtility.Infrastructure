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

namespace ArkUtility.Infrastructure.Extensions
{
    public static class StreamExtensions
    {
        /// <summary>
        /// Valid for files up to 2GB. 
        /// </summary>
        /// <param name="stream"></param>
        /// <param name="bufferSize"></param>
        /// <returns></returns>
        public static byte[] ReadSteamIntoBytes(this Stream stream, int bufferSize)
        {
            MemoryStream ms = new MemoryStream();
            stream.CopyTo(ms, bufferSize);
            return ms.ToArray();
        }

        /// <summary>
        /// Valid for files up to 2GB. Does not support higher byte counts.
        /// </summary>
        /// <param name="stream"></param>
        /// <param name="bufferSize"></param>
        /// <returns></returns>
        public static byte[] ReadSteamIntoBytes(this Stream stream, long bufferSize)
        {
            try
            {
                var intBuffer = Convert.ToInt32(bufferSize);
                return stream.ReadSteamIntoBytes(intBuffer);
            }
            catch (OverflowException)
            {
                throw new NotSupportedException("ReadSteamIntoBytes() does not support  files > 2GB Please use alternate methods for files of this size. ");
            }

        }
    }
}