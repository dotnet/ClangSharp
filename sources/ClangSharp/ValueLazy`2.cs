// Copyright (c) .NET Foundation and Contributors. All Rights Reserved. Licensed under the MIT License (MIT). See License.md in the repository root for more information.

// This file includes code based on the Lazy<T> class from https://github.com/dotnet/runtime/
// The original code is Copyright © .NET Foundation and Contributors. All rights reserved. Licensed under the MIT License (MIT).

using System;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;

namespace ClangSharp;

// A lazily-computed value whose factory is a static function pointer taking the
// owning instance. Unlike a Func<T>-based lazy, this allocates nothing per
// instance -- the factory is a static delegate*<TSelf, T> rather than a captured
// delegate -- so it can be used freely for members that are commonly accessed.
internal unsafe struct ValueLazy<TSelf, [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicParameterlessConstructor)] T>
    where TSelf : class
{
    private delegate*<TSelf, T> _factory;
    private T _value;

    public ValueLazy(delegate*<TSelf, T> factory)
    {
        Unsafe.SkipInit(out this);
        _factory = factory;
    }

    public readonly bool IsValueCreated => _factory is null;

    public T GetValue(TSelf self)
    {
        if (_factory is not null)
        {
            _value = _factory(self);
            _factory = null;
        }
        return _value;
    }
}
