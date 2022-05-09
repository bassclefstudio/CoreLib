﻿namespace BassClefStudio.CoreLib.Helpers
{
    /// <summary>
    /// A class containing extension methods for <see cref="IList{T}"/> and <see cref="IEnumerable{T}"/>.
    /// </summary>
    public static class CollectionExtensions
    {
        #region RangeActions

        /// <summary>
        /// Adds a collection of items to an <see cref="IList{T}"/>.
        /// </summary>
        /// <param name="list">The list to add <paramref name="items"/> to.</param>
        /// <param name="items">An <see cref="IEnumerable{T}"/> of items to add to the list.</param>
        public static void AddRange<T>(this IList<T> list, IEnumerable<T> items)
        {
            foreach (var item in items)
            {
                list.Add(item);
            }
        }

        /// <summary>
        /// Removes a collection of items from an <see cref="IList{T}"/>.
        /// </summary>
        /// <param name="list">The list to remove <paramref name="items"/> from.</param>
        /// <param name="items">An <see cref="IEnumerable{T}"/> of items to remove from the list.</param>
        public static void RemoveRange<T>(this IList<T> list, IEnumerable<T> items)
        {
            foreach (var item in items)
            {
                list.Remove(item);
            }
        }

        #endregion
        #region Sorting

        /// <summary>
        /// Performs a binary search on the specified collection.
        /// </summary>
        /// <typeparam name="TItem">The type of the item.</typeparam>
        /// <param name="list">The list to be searched.</param>
        /// <param name="value">The value to search for.</param>
        /// <returns></returns>
        public static int BinarySearch<TItem>(this IList<TItem> list, TItem value)
        {
            return BinarySearch(list, value, Comparer<TItem>.Default);
        }

        /// <summary>
        /// Performs a binary search on the specified collection.
        /// </summary>
        /// <typeparam name="TItem">The type of the item.</typeparam>
        /// <param name="list">The list to be searched.</param>
        /// <param name="value">The value to search for.</param>
        /// <param name="comparer">The comparer that is used to compare the value with the list items.</param>
        /// <returns></returns>
        public static int BinarySearch<TItem>(this IList<TItem> list, TItem value,
            IComparer<TItem> comparer)
        {
            return list.BinarySearch(value, comparer.Compare);
        }

        /// <summary>
        /// Performs a binary search on the specified collection.
        /// </summary>
        /// <typeparam name="TItem">The type of the item.</typeparam>
        /// <typeparam name="TSearch">The type of the searched item.</typeparam>
        /// <param name="list">The list to be searched.</param>
        /// <param name="value">The value to search for.</param>
        /// <param name="comparer">The comparer that is used to compare the value with the list items.</param>
        /// <returns></returns>
        public static int BinarySearch<TItem, TSearch>(this IList<TItem> list,
            TSearch value, Func<TSearch, TItem, int> comparer)
        {
            if (list == null)
            {
                throw new ArgumentNullException("list");
            }
            if (comparer == null)
            {
                throw new ArgumentNullException("comparer");
            }

            int lower = 0;
            int upper = list.Count - 1;

            while (lower <= upper)
            {
                int middle = lower + (upper - lower) / 2;
                int comparisonResult = comparer(value, list[middle]);
                if (comparisonResult < 0)
                {
                    upper = middle - 1;
                }
                else if (comparisonResult > 0)
                {
                    lower = middle + 1;
                }
                else
                {
                    return middle;
                }
            }

            return ~lower;
        }

        /// <summary>
        /// Adds an item to an <see cref="IList{T}"/> in a way so that the list remains in order.
        /// </summary>
        /// <typeparam name="T">The type of comparable items in the <see cref="IList{T}"/>.</typeparam>
        /// <param name="list">An initially sorted/ordered list of <typeparamref name="T"/> items.</param>
        /// <param name="item">The new <typeparamref name="T"/> item to insert into the <paramref name="list"/>.</param>
        public static void AddSorted<T>(this IList<T> list, T item) => AddSorted(list, item, Comparer<T>.Default);

        /// <summary>
        /// Adds an item to an <see cref="IList{T}"/> in a way so that the list remains in order.
        /// </summary>
        /// <typeparam name="T">The type of comparable items in the <see cref="IList{T}"/>.</typeparam>
        /// <param name="list">An initially sorted/ordered list of <typeparamref name="T"/> items.</param>
        /// <param name="item">The new <typeparamref name="T"/> item to insert into the <paramref name="list"/>.</param>
        /// <param name="comparer">The comparer that is used to compare <paramref name="item"/> with the items in <paramref name="list"/>.</param>
        public static void AddSorted<T>(this IList<T> list, T item, IComparer<T> comparer) => AddSorted(list, item, comparer.Compare);

        /// <summary>
        /// Adds an item to an <see cref="IList{T}"/> in a way so that the list remains in order.
        /// </summary>
        /// <typeparam name="T">The type of comparable items in the <see cref="IList{T}"/>.</typeparam>
        /// <param name="list">An initially sorted/ordered list of <typeparamref name="T"/> items.</param>
        /// <param name="item">The new <typeparamref name="T"/> item to insert into the <paramref name="list"/>.</param>
        /// <param name="comparer">The comparer that is used to compare <paramref name="item"/> with the items in <paramref name="list"/>.</param>
        public static void AddSorted<T>(this IList<T> list, T item, Func<T, T, int> comparer)
        {
            if (list.Count == 0)
            {
                list.Add(item);
                return;
            }
            if (comparer(list[list.Count - 1], item) <= 0)
            {
                list.Add(item);
                return;
            }
            if (comparer(list[0], item) >= 0)
            {
                list.Insert(0, item);
                return;
            }
            int index = list.BinarySearch(item, comparer);
            if (index < 0)
                index = ~index;
            list.Insert(index, item);
        }

        #endregion
        #region Sync

        private static Func<T1,T2, bool> GetEqualityFunc<T1,T2>(Func<T1,T2, bool> func)
        {
            return (a, b) => a != null && b != null && func(a, b);
        }

        /// <summary>
        /// Synchronizes the items of an <see cref="IList{T}"/> with the items in another <see cref="IEnumerable{T}"/> by adding and removing related items from the list.
        /// </summary>
        /// <param name="list">This <see cref="IList{T}"/>, which will be synchronized.</param>
        /// <param name="newData">A collection of new data to use to update <paramref name="list"/>.</param>
        /// <param name="equalityFunc">A function that returns a <see cref="bool"/> indicating whether two items of type <typeparamref name="T"/> are equal for the purposes of the operation.</param>
        /// <param name="replaceFunc">A function that, given the item of type <typeparamref name="T"/> in the <paramref name="list"/> and the corresponding item in <paramref name="newData"/>, returns the new item to include in <paramref name="list"/>.</param>
        /// <param name="isDestructive">A <see cref="bool"/> indicating if items should be removed from <paramref name="list"/> if they do not occur in <paramref name="newData"/>.</param>
        public static void Sync<T>(this IList<T> list, IEnumerable<T> newData, Func<T, T, bool> equalityFunc, Func<T, T, T> replaceFunc, bool isDestructive = true)
        {
            var toAdd = newData.Where(d => !list.Any(i => GetEqualityFunc(equalityFunc)(d, i))).ToList();
            if (isDestructive)
            {
                var toRemove = list.Where(d => !newData.Any(i => GetEqualityFunc(equalityFunc)(d, i))).ToList();
                list.RemoveRange(toRemove);
            }
            list.AddRange(toAdd);

            for (int i = 0; i < list.Count; i++)
            {
                var item = list[i];
                if (newData.Any(d => GetEqualityFunc(equalityFunc)(d, item)))
                {
                    list[i] = replaceFunc(item, newData.First(d => GetEqualityFunc(equalityFunc)(d, item)));
                }
            }
        }

        /// <summary>
        /// Synchronizes the items of an <see cref="IList{T}"/> with the items in another <see cref="IEnumerable{T}"/> by adding and removing related items from the list.
        /// </summary>
        /// <param name="list">This <see cref="IList{T}"/>, which will be synchronized.</param>
        /// <param name="newData">A collection of new data to use to update <paramref name="list"/>.</param>
        /// <param name="isDestructive">A <see cref="bool"/> indicating if items should be removed from <paramref name="list"/> if they do not occur in <paramref name="newData"/>.</param>
        public static void Sync<T>(this IList<T> list, IEnumerable<T> newData, bool isDestructive = true) => Sync<T>(list, newData, (a, b) => Equals(a, b), isDestructive);
        /// <summary>
        /// Synchronizes the items of an <see cref="IList{T}"/> with the items in another <see cref="IEnumerable{T}"/> by adding and removing related items.
        /// </summary>
        /// <param name="list">This <see cref="IList{T}"/>, which will be synchronized.</param>
        /// <param name="newData">A collection of new data to use to update <paramref name="list"/>.</param>
        /// <param name="equalityFunc">A function that returns a <see cref="bool"/> indicating whether two items of type <typeparamref name="T"/> are equal for the purposes of the operation.</param>
        /// <param name="isDestructive">A <see cref="bool"/> indicating if items should be removed from <paramref name="list"/> if they do not occur in <paramref name="newData"/>.</param>
        public static void Sync<T>(this IList<T> list, IEnumerable<T> newData, Func<T, T, bool> equalityFunc, bool isDestructive = true)
        {
            var toAdd = newData.Where(d => !list.Any(i => GetEqualityFunc(equalityFunc)(d, i))).ToList();
            if (isDestructive)
            {
                var toRemove = list.Where(d => !newData.Any(i => GetEqualityFunc(equalityFunc)(d, i))).ToList();
                list.RemoveRange(toRemove);
            }
            list.AddRange(toAdd);
        }

        /// <summary>
        /// Synchronizes the items of an <see cref="IList{T}"/> with the items in another <see cref="IEnumerable{T}"/> by adding and removing related items.
        /// </summary>
        /// <typeparam name="T">The type of items in both collections.</typeparam>
        /// <typeparam name="TKey">The type of the items' <see cref="IIdentifiable{T}.Id"/>.</typeparam>
        /// <param name="list">This <see cref="IList{T}"/>, which will be synchronized.</param>
        /// <param name="newData">A collection of new data to use to update <paramref name="list"/>.</param>
        /// <param name="isDestructive">A <see cref="bool"/> indicating if items should be removed from <paramref name="list"/> if they do not occur in <paramref name="newData"/>.</param>
        public static void Sync<T, TKey>(this IList<T> list, IEnumerable<T> newData, bool isDestructive = true) where T : IIdentifiable<TKey> where TKey : IEquatable<TKey>
            => Sync(list, newData, (a, b) => a.Id.Equals(b.Id), isDestructive);

        /// <summary>
        /// Synchronizes the items of an <see cref="IList{T}"/> with the items in another <see cref="IEnumerable{T}"/> by adding and removing related items.
        /// </summary>
        /// <typeparam name="T1">The type of the items in <paramref name="list"/>.</typeparam>
        /// <typeparam name="T2">The type of the items in <paramref name="newData"/>.</typeparam>
        /// <typeparam name="TKey">The type of the items' <see cref="IIdentifiable{T}.Id"/>.</typeparam>
        /// <param name="list">This <see cref="IList{T}"/>, which will be synchronized.</param>
        /// <param name="newData">A collection of new data to use to update <paramref name="list"/>.</param>
        /// <param name="createFunc">A function that, given the item of type <typeparamref name="T2"/> in the <paramref name="newData"/>, creates a new item of type <typeparamref name="T1"/> to include in <paramref name="list"/>.</param>
        /// <param name="isDestructive">A <see cref="bool"/> indicating if items should be removed from <paramref name="list"/> if they do not occur in <paramref name="newData"/>.</param>
        public static void Sync<T1, T2, TKey>(this IList<T1> list, IEnumerable<T2> newData, Func<T2, T1> createFunc, bool isDestructive = true) where T1 : IIdentifiable<TKey> where T2 : IIdentifiable<TKey> where TKey : IEquatable<TKey>
            => Sync(list, newData, (a, b) => a.Id.Equals(b.Id), createFunc, isDestructive);

        /// <summary>
        /// Synchronizes the items of an <see cref="IList{T}"/> with the items in another <see cref="IEnumerable{T}"/> by adding and removing related items.
        /// </summary>
        /// <typeparam name="T1">The type of the items in <paramref name="list"/>.</typeparam>
        /// <typeparam name="T2">The type of the items in <paramref name="newData"/>.</typeparam>
        /// <param name="list">This <see cref="IList{T}"/>, which will be synchronized.</param>
        /// <param name="newData">A collection of new data to use to update <paramref name="list"/>.</param>
        /// <param name="equalityFunc">A function that returns a <see cref="bool"/> indicating whether an item of type <typeparamref name="T1"/> and an item of type <typeparamref name="T2"/> are equal for the purposes of the operation.</param>
        /// <param name="createFunc">A function that, given the item of type <typeparamref name="T2"/> in the <paramref name="newData"/>, creates a new item of type <typeparamref name="T1"/> to include in <paramref name="list"/>.</param>
        /// <param name="isDestructive">A <see cref="bool"/> indicating if items should be removed from <paramref name="list"/> if they do not occur in <paramref name="newData"/>.</param>
        public static void Sync<T1, T2>(this IList<T1> list, IEnumerable<T2> newData, Func<T1, T2, bool> equalityFunc, Func<T2, T1> createFunc, bool isDestructive = true)
        {
            var toAdd = newData.Where(d => !list.Any(i => GetEqualityFunc(equalityFunc)(i, d))).ToList();
            if (isDestructive)
            {
                var toRemove = list.Where(d => !newData.Any(i => GetEqualityFunc(equalityFunc)(d, i))).ToList();
                list.RemoveRange(toRemove);
            }

            if (createFunc != null)
            {
                list.AddRange(toAdd.Select(a => createFunc(a)));
            }
        }

        #endregion
        #region Grouping

        /// <summary>
        /// Takes an <see cref="IEnumerable{T}"/> and returns a collection of <see cref="IEnumerable{T}"/>s with each containing at most <paramref name="chunkSize"/> items from the original collection.
        /// </summary>
        /// <param name="source">The source collection to group.</param>
        /// <param name="chunkSize">The maximum size of the chunks taken.</param>
        public static IEnumerable<IEnumerable<T>> ChunkBy<T>(this IEnumerable<T> source, int chunkSize)
        {
            return source
                .Select((x, i) => new { Index = i, Value = x })
                .GroupBy(x => x.Index / chunkSize)
                .Select(x => x.Select(v => v.Value));
        }

        #endregion
        #region Transpose

        /// <summary>
        /// Swaps the indices for an <see cref="IEnumerable{T}"/> of <see cref="IEnumerable{T}"/>s and returns the result as a different <see cref="IEnumerable{T}"/> of <see cref="IEnumerable{T}"/>s.
        /// </summary>
        public static IEnumerable<IEnumerable<T>> Transpose<T>(this IEnumerable<IEnumerable<T>> source)
        {
            if (source == null)
            {
                throw new ArgumentNullException("source");
            }

            if (!source.Any())
            {
                return Enumerable.Empty<IEnumerable<T>>();
            }

            return TransposeCore(source);
        }

        static IEnumerable<IEnumerable<T>> TransposeCore<T>(this IEnumerable<IEnumerable<T>> source)
        {
            var enumerators = source.Select(x => x.GetEnumerator()).ToArray();
            try
            {
                while (enumerators.All(x => x.MoveNext()))
                {
                    yield return enumerators.Select(x => x.Current).ToArray();
                }
            }
            finally
            {
                foreach (var enumerator in enumerators)
                    enumerator.Dispose();
            }
        }

        #endregion
    }
}
