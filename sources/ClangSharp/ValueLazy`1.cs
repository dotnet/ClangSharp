// Copyright (c) .NET Foundation and Contributors. All Rights Reserved. Licensed under the MIT License (MIT). See License.md in the repository root for more information.

// This file includes code based on the Lazy<T> class from https://github.com/dotnet/runtime/
// The original code is Copyright © .NET Foundation and Contributors. All rights reserved. Licensed under the MIT License (MIT).

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;

namespace ClangSharp;

internal partial struct ValueLazy<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicParameterlessConstructor)] T> : IEquatable<ValueLazy<T>>
{
    private Func<T>? _factory;
    private T _value;

    public ValueLazy(Func<T> factory)
    {
        Unsafe.SkipInit(out this);
        Reset(factory);
    }

    public readonly bool IsValueCreated => _factory is null;

    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    public T Value
    {
        get
        {
            if (_factory is Func<T> factory)
            {
                _value = factory();
                _factory = null;
            }
            return _value;
        }
    }

    public static bool operator ==(ValueLazy<T> left, ValueLazy<T> right)
        => (left._factory == right._factory)
        && EqualityComparer<T>.Default.Equals(left._value, right._value);

    public static bool operator !=(ValueLazy<T> left, ValueLazy<T> right) => !(left == right);

    public override readonly bool Equals([NotNullWhen(true)] object? obj) => (obj is ValueLazy<T> other) && Equals(other);

    public readonly bool Equals(ValueLazy<T> other) => this == other;

    public override readonly int GetHashCode() => HashCode.Combine(_factory, _value);

    public void Reset(Func<T> factory)
    {
        ArgumentNullException.ThrowIfNull(factory);
        _factory = factory;
    }

    public override readonly string ToString()
    {
        if (!IsValueCreated || (_value is not T value))
        {
            return string.Empty;
        }
        return value.ToString() ?? string.Empty;
    }
}
