﻿using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Common.Extensions
{
    public static class ListExtensions
    {
        public static async Task ForEachAsync<T>(this List<T> list, Func<T, Task> func)
        {
            foreach (var value in list)
            {
                await func(value);
            }
        }
    }
}
