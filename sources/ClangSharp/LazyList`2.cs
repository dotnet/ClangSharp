// Copyright © Tanner Gooding and Contributors. Licensed under the MIT License (MIT). See License.md in the repository root for more information.

using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace ClangSharp;

internal sealed class LazyList<T, [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicParameterlessConstructor)] TBase> : IList<T>, IReadOnlyList<T>
    where T : class, TBase
    where TBase : class
{
    internal readonly TBase[] _items;
    internal readonly Func<int, TBase> _valueFactory;

    private readonly int _start;
    private readonly int _count;

    public LazyList(LazyList<TBase> list, int skip = -1, int take = -1)
    {
        skip = (skip < 0) ? 0 : skip;
        take = (take < 0) ? (list.Count - skip) : take;

        _items = list._items;
        _valueFactory = list._valueFactory;

        _start = skip;
        _count = take;
    }

    public T this[int index]
    {
        get
        {
            var items = _items.AsSpan(_start, _count);
            var item = items[index];

            if (item is null)
            {
                item = _valueFactory(index + _start);
                items[index] = item;
            }

            return (T)item;
        }
    }

    public int Count => _count;

    public bool IsReadOnly => true;

    public bool Contains(T item) => IndexOf(item) >= 0;

    public void CopyTo(T[] array, int arrayIndex)
    {
        var items = _items.AsSpan(_start, _count);
        ArgumentNullException.ThrowIfNull(array);

        if ((arrayIndex < 0) || (arrayIndex > array.Length) || ((array.Length - arrayIndex) < items.Length))
        {
            throw new ArgumentOutOfRangeException(nameof(arrayIndex));
        }

        for (var i = 0; i < _count; i++)
        {
            var currentItem = items[i];

            if (currentItem is null)
            {
                currentItem = _valueFactory(i + _start);
                items[i] = currentItem;
            }

            array[arrayIndex + i] = (T)currentItem;
        }
    }

    public Enumerator GetEnumerator() => new Enumerator(this);

    public int IndexOf(T item)
    {
        var items = _items.AsSpan(_start, _count);

        for (var i = 0; i < items.Length; i++)
        {
            var currentItem = items[i];

            if (currentItem is null)
            {
                currentItem = _valueFactory(i + _start);
                items[i] = currentItem;
            }

            if (EqualityComparer<T>.Default.Equals((T)currentItem, item))
            {
                return i;
            }
        }

        return -1;
    }

    T IList<T>.this[int index]
    {
        get
        {
            return this[index];
        }

        set
        {
            throw new NotSupportedException();
        }
    }

    void ICollection<T>.Add(T item) => throw new NotSupportedException();

    void ICollection<T>.Clear() => throw new NotSupportedException();

    bool ICollection<T>.Remove(T item) => throw new NotSupportedException();

    void IList<T>.Insert(int index, T item) => throw new NotSupportedException();

    void IList<T>.RemoveAt(int index) => throw new NotSupportedException();

    IEnumerator<T> IEnumerable<T>.GetEnumerator() => GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    public struct Enumerator : IEnumerator<T>
    {
        private readonly LazyList<T, TBase> _list;

        private int _index;
        private T? _current;

        internal Enumerator(LazyList<T, TBase> list)
        {
            _list = list;
        }

        public readonly void Dispose()
        {
        }

        public bool MoveNext()
        {
            var localList = _list;

            if ((uint)_index < (uint)localList.Count)
            {
                _current = localList[_index];
                _index++;
                return true;
            }

            _current = default;
            _index = -1;
            return false;
        }

        public readonly T Current => _current!;

        readonly object? IEnumerator.Current => Current;

        void IEnumerator.Reset()
        {
            _index = 0;
            _current = default;
        }
    }
}
