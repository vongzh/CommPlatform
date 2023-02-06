/**
 * wangzheng
 * vongzh@qq.com
 * 2020/07/04
 * */
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace Zk.HotelPlatform.Utils.Concurrent
{
    public class ConcurrentHashSet<T> : IEnumerable<T>, IEnumerable, ICollection<T>, IDisposable
    {
        private readonly HashSet<T> _hashSet = new HashSet<T>();

        private readonly ReaderWriterLockSlim _lock = new ReaderWriterLockSlim(LockRecursionPolicy.SupportsRecursion);

        public bool Add(T item)
        {
            return this.WriteLock<bool>(() => this._hashSet.Add(item));
        }

        void ICollection<T>.Add(T item)
        {
            this.Add(item);
        }

        public void Clear()
        {
            this.WriteLock(delegate ()
            {
                this._hashSet.Clear();
            });
        }

        public bool Contains(T item)
        {
            return this.ReadLock<bool>(() => this._hashSet.Contains(item));
        }

        public void CopyTo(T[] array, int arrayIndex)
        {
            this.ReadLock(delegate ()
            {
                this._hashSet.CopyTo(array, arrayIndex);
            });
        }

        public bool Remove(T item)
        {
            return this.WriteLock<bool>(() => this._hashSet.Remove(item));
        }

        public int Count
        {
            get
            {
                return this.ReadLock<int>(() => this._hashSet.Count);
            }
        }

        bool ICollection<T>.IsReadOnly
        {
            get
            {
                return false;
            }
        }

        public void Dispose()
        {
            ReaderWriterLockSlim @lock = this._lock;
            if (@lock == null)
            {
                return;
            }
            @lock.Dispose();
        }

        public IEnumerator<T> GetEnumerator()
        {
            return this.ReadLock<List<T>.Enumerator>(() => this._hashSet.ToList<T>().GetEnumerator());
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }

        private TResult ReadLock<TResult>(Func<TResult> func)
        {
            this._lock.EnterReadLock();
            TResult result;
            try
            {
                result = func();
            }
            finally
            {
                if (this._lock.IsReadLockHeld)
                {
                    this._lock.ExitReadLock();
                }
            }
            return result;
        }

        private void ReadLock(Action func)
        {
            this._lock.EnterReadLock();
            try
            {
                func();
            }
            finally
            {
                if (this._lock.IsReadLockHeld)
                {
                    this._lock.ExitReadLock();
                }
            }
        }
        private TResult WriteLock<TResult>(Func<TResult> func)
        {
            this._lock.EnterWriteLock();
            TResult result;
            try
            {
                result = func();
            }
            finally
            {
                if (this._lock.IsWriteLockHeld)
                {
                    this._lock.ExitWriteLock();
                }
            }
            return result;
        }

        private void WriteLock(Action func)
        {
            this._lock.EnterWriteLock();
            try
            {
                func();
            }
            finally
            {
                if (this._lock.IsWriteLockHeld)
                {
                    this._lock.ExitWriteLock();
                }
            }
        }
    }
}
