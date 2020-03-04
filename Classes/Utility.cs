using System;
using System.Linq;
using System.Collections.Generic;

namespace QuestGenerator
{
    class Utility
    {
        public static IEnumerable<Array> GetArraysInObject<T>(T obj)
            => typeof(T).GetProperties()
                .Where(p => p.PropertyType.IsArray)
                .Select(p => (Array)p.GetValue(obj));

        public static int GetFullAttributeCount<T>(T[] a, T[] b)
            => a.Union(b).Count() * typeof(T).GetProperties().Length;

        public static int GetCommonAttributeCount<T>(T[] a, T[] b)
        {
            int commonAttributeCount = 0;

            int attributeCount = typeof(T).GetProperties().Length;

            for (int i = 0; i < Math.Min(a.Length, b.Length); ++i)
                commonAttributeCount += GetCommonAttributeCount(a[i], b[i]);

            if (a.Length != b.Length)
                commonAttributeCount += Math.Abs(a.Length - b.Length) * attributeCount;

            return commonAttributeCount;
        }

        public static int GetCommonAttributeCount<T>(T a, T b)
        {
            if (a.Equals(b))
                return typeof(T).GetProperties().Length;
            else
                return typeof(T).GetProperties().Where(p => !p.GetValue(a).Equals(p.GetValue(b))).Count();

        }
    }
}
