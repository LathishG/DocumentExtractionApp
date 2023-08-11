﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DocumentProcess.Common
{
    public static class Extensions
    {
        public static List<List<T>> partition<T>(this List<T> values, int chunkSize)
        {            
            return values.Select((x, i) => new { Index = i, Value = x })
            .GroupBy(x => x.Index / chunkSize)
            .Select(x => x.Select(v => v.Value).ToList())
            .ToList();
        }
    }
}
