﻿using System;
using System.Collections.Generic;
using System.Linq;

namespace Milky.Utilities
{
    internal class Library<T>
    {
        public readonly List<KeyValuePair<int, T>> Items = new List<KeyValuePair<int, T>>();
        public readonly List<int> Borrowed = new List<int>();

        private readonly object _locker = new object();
        private readonly Random _random = new Random();

        public void Add(T item)
        {
            lock (_locker)
            {
                Items.Add(new KeyValuePair<int, T>(Items.Count, item));
            }
        }

        public void Fill(int till)
        {
            lock (_locker)
            {
                if (Items.Count == 0)
                {
                    throw new Exception("Library contains 0 items");
                }

                int beforelItemsCount = Items.Count;

                for (int i = 0; i < till - beforelItemsCount; i++)
                {
                    Add(Items[i].Value);
                }
            }
        }

        public void RandomlyFill(int till)
        {
            lock (_locker)
            {
                if (Items.Count == 0)
                {
                    throw new Exception("Library contains 0 items");
                }

                int beforelItemsCount = Items.Count;

                while (Items.Count < till)
                {
                    foreach (var item in Items.GetRange(0, beforelItemsCount).OrderBy(i => _random.Next()).Take(till - Items.Count))
                    {
                        Add(item.Value);
                    }
                }
            }
        }

        public void Remove(KeyValuePair<int, T> item)
        {
            lock (_locker)
            {
                Borrowed.Remove(item.Key);

                Items.Remove(item);
            }
        }

        public void Replace(List<T> items)
        {
            lock (_locker)
            {
                Items.Clear();
                Borrowed.Clear();

                items.ForEach(item => Add(item));
            }
        }

        public bool TryBorrowFirst(out KeyValuePair<int, T> item)
        {
            lock (_locker)
            {
                if (Borrowed.Count == Items.Count)
                {
                    item = new KeyValuePair<int, T>();

                    return false;
                }

                item = Items.First(i => !Borrowed.Contains(i.Key));

                Borrowed.Add(item.Key);

                return true;
            }
        }

        public bool TryBorrowRandom(out KeyValuePair<int, T> item)
        {
            lock (_locker)
            {
                if (Borrowed.Count == Items.Count)
                {
                    item = new KeyValuePair<int, T>();

                    return false;
                }

                item = Items.Where(i => !Borrowed.Contains(i.Key)).OrderBy(i => _random.Next()).First();

                Borrowed.Add(item.Key);

                return true;
            }
        }

        public void Return(KeyValuePair<int, T> item)
        {
            lock (_locker)
            {
                Borrowed.Remove(item.Key);
            }
        }
    }
}
