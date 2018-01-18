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
    public static class EnumerableExtensions
    {
        public static void ForEach<T>(this IEnumerable<T> list, Action<T> action)
        {
            if (list == null || action == null)
                return;
            foreach (T item in list)
                action(item);
        }

        public static void ForEach<T>(this IEnumerable<T> list, Action<T, int> action)
        {
            if (list == null || action == null)
                return;
            int i = 0;
            foreach (T item in list)
            {
                action(item, i);
                i++;
            }
        }
    }
}
