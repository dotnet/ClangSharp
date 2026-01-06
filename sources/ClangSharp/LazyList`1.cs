// Copyright © Tanner Gooding and Contributors. Licensed under the MIT License (MIT). See License.md in the repository root for more information.

using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace ClangSharp;

internal sealed class LazyList<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicParameterlessConstructor)] T> : IList<T>, IReadOnlyList<T>
    where T : class
{
    internal readonly T[] _items;
    internal readonly Func<int, T>? _valueFactory;
    internal readonly Func<int, T?, T>? _valueFactoryWithPreviousValue;

    public static readonly LazyList<T> Empty = new LazyList<T>(0, _ => null!);

    public LazyList(int count, Func<int, T> valueFactory)
    {
        _items = (count <= 0) ? [] : new T[count];
        _valueFactory = valueFactory;
        _valueFactoryWithPreviousValue = null;
    }

    public LazyList(int count, Func<int, T?, T> valueFactoryWithPreviousValue)
    {
        _items = (count <= 0) ? [] : new T[count];
        _valueFactory = null;
        _valueFactoryWithPreviousValue = valueFactoryWithPreviousValue;
    }

    public T this[int index]
    {
        get
        {
            var items = _items.AsSpan();
            var item = items[index];

            if (item is null)
            {
                if (_valueFactoryWithPreviousValue is not null)
                {
                    item = _valueFactoryWithPreviousValue(index, index == 0 ? null : _items[index - 1]);
                } else {
                    item = _valueFactory!.Invoke(index);
                }
                items[index] = item;
            }

            return item;
        }
    }

    public int Count => _items.Length;

    public bool IsReadOnly => true;

    public bool Contains(T item) => IndexOf(item) >= 0;

    public void CopyTo(T[] array, int arrayIndex)
    {
        var items = _items;
        ArgumentNullException.ThrowIfNull(array);

        if ((arrayIndex < 0) || (arrayIndex > array.Length) || ((array.Length - arrayIndex) < items.Length))
        {
            throw new ArgumentOutOfRangeException(nameof(arrayIndex));
        }

        for (var i = 0; i < items.Length; i++)
        {
            var currentItem = this[i];
            array[arrayIndex + i] = currentItem;
        }
    }

    public Enumerator GetEnumerator() => new Enumerator(this);

    public int IndexOf(T item)
    {
        var items = _items;

        for (var i = 0; i < items.Length; i++)
        {
            var currentItem = this[i];
            if (EqualityComparer<T>.Default.Equals(currentItem, item))
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
        private readonly LazyList<T> _list;

        private int _index;
        private T? _current;

        internal Enumerator(LazyList<T> list)
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
