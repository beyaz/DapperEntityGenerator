using System;
using System.Collections.Generic;
using System.Globalization;
using System.Threading;

namespace DapperEntityGenerator
{
    public static class Extensions
    {
        #region Public Methods
        public static IReadOnlyList<T> Loop<T>(Func<IReadOnlyList<T>> func, Action<T> process, Action<int> percentOfComplete)
        {
            return Loop(func(), process, percentOfComplete);
        }

        public static IReadOnlyList<T> Loop<T>(IReadOnlyList<T> list, Action<T> process, Action<int> percentOfComplete)
        {
            int calculatePercent(int i)
            {
                return (int) Math.Ceiling(100M / list.Count * (i + 1));
            }

            for (var i = 0; i < list.Count; i++)
            {
                process(list[i]);

                percentOfComplete(calculatePercent(i));
            }

            return list;
        }

        public static void StartThread(ThreadStart action)
        {
            new Thread(action).Start();
        }

        /// <summary>
        ///     Returns a copy of this string converted to lowercase, using the casing rules of 'Turkish' culture
        /// </summary>
        public static string ToLowerTR(this string value)
        {
            return value.ToLower(new CultureInfo("tr-TR"));
        }
        #endregion
    }
}