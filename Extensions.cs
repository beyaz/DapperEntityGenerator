using System;
using System.Collections.Generic;
using System.Threading;
using System.Diagnostics.Contracts;
using Microsoft.SqlServer.Management.Smo;
using System.Globalization;

namespace DapperEntityGenerator
{
    public static class Extensions
    {

        /// <summary>
        ///     Returns a copy of this string converted to lowercase, using the casing rules of 'English' culture
        /// </summary>
        public static string ToLowerEN(this string value)
        {
            return value.ToLower(new CultureInfo("en-US"));
        }

        /// <summary>
        ///     Returns a copy of this string converted to lowercase, using the casing rules of 'Turkish' culture
        /// </summary>
        public static string ToLowerTR(this string value)
        {
            return value.ToLower(new CultureInfo("tr-TR"));
        }

        /// <summary>
        ///     Returns a copy of this string converted to uppercase, using the casing rules of 'English' culture
        /// </summary>
        public static string ToUpperEN(this string value)
        {
            return value.ToUpper(new CultureInfo("en-US"));
        }

        /// <summary>
        ///     Returns a copy of this string converted to uppercase, using the casing rules of Turkish culture
        /// </summary>
        public static string ToUpperTR(this string value)
        {
            return value.ToUpper(new CultureInfo("tr-TR"));
        }


        
        

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
        #endregion

        
            #region fun
            /// <summary>
            ///     Funs the specified f.
            /// </summary>
            [Pure]
            public static Func<R> Fun<R>(Func<R> f) => f;

            /// <summary>
            ///     Funs the specified f.
            /// </summary>
            [Pure]
            public static Func<T1, R> Fun<T1, R>(Func<T1, R> f) => f;

            /// <summary>
            ///     Funs the specified f.
            /// </summary>
            [Pure]
            public static Func<T1, T2, R> Fun<T1, T2, R>(Func<T1, T2, R> f) => f;

            /// <summary>
            ///     Funs the specified f.
            /// </summary>
            [Pure]
            public static Func<T1, T2, T3, R> Fun<T1, T2, T3, R>(Func<T1, T2, T3, R> f) => f;
            #endregion

            #region fun actions
            /// <summary>
            ///     Funs the specified f.
            /// </summary>
            [Pure]
            public static Action<T> Fun<T>(Action<T> f) => f;

            /// <summary>
            ///     Funs the specified f.
            /// </summary>
            [Pure]
            public static Action Fun(Action f) => f;

            /// <summary>
            ///     Funs the specified f.
            /// </summary>
            [Pure]
            public static Action<T1, T2> Fun<T1, T2>(Action<T1, T2> f) => f;

            /// <summary>
            ///     Funs the specified f.
            /// </summary>
            [Pure]
            public static Action<T1, T2, T3> Fun<T1, T2, T3>(Action<T1, T2, T3> f) => f;
            #endregion


            public static IEnumerable<IndexedColumn> ToEnumeration(this IndexedColumnCollection collection)
            {
                foreach (IndexedColumn item in collection)
                {
                    yield return item;
                }
            }

            public static IEnumerable<Index> ToEnumeration(this IndexCollection collection)
            {
                foreach (Index item in collection)
                {
                    yield return item;
                }
            }

            

            public static IEnumerable<Column> ToEnumeration(this ColumnCollection collection)
            {
                foreach (Column item in collection)
                {
                    yield return item;
                }
            }

            
    }
}