using System;
using System.Collections.Generic;
using System.Globalization;
using System.Threading;
using Microsoft.SqlServer.Management.Smo;

namespace DapperEntityGenerator
{
    delegate void LoopTrace<T>(T item, int count, int index, int completePercent);

    /// <summary>
    ///     The extensions
    /// </summary>
    static class Extensions
    {
        #region Public Methods
        /// <summary>
        ///     Loops the specified function.
        /// </summary>
        public static IReadOnlyList<T> Loop<T>(Func<IReadOnlyList<T>> func, Action<T> process, LoopTrace<T> trace)
        {
            var list = func();

            int calculatePercent(int i)
            {
                return (int) Math.Ceiling(100M / list.Count * (i + 1));
            }

            for (var i = 0; i < list.Count; i++)
            {
                trace(list[i], list.Count, i, calculatePercent(i));

                process(list[i]);
            }

            return list;
        }

        public static IEnumerable<Column> ToEnumerable(this ColumnCollection columnCollection)
        {
            foreach (Column column in columnCollection)
            {
                yield return column;
            }
        }

        /// <summary>
        ///     Starts the thread.
        /// </summary>
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