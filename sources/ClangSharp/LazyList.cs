// Copyright © Tanner Gooding and Contributors. Licensed under the MIT License (MIT). See License.md in the repository root for more information.

using System;
using System.Diagnostics.CodeAnalysis;

namespace ClangSharp;

internal static class LazyList
{
    public static LazyList<T> Create<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicParameterlessConstructor)] T>(int count, Func<int, T> valueFactory)
        where T : class
    {
        if (count <= 0)
        {
            return LazyList<T>.Empty;
        }
        return new LazyList<T>(count, valueFactory);
    }

    public static LazyList<T> Create<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicParameterlessConstructor)] T>(int count, Func<int, T?, T> valueFactory)
        where T : class
    {
        if (count <= 0)
        {
            return LazyList<T>.Empty;
        }
        return new LazyList<T>(count, valueFactory);
    }

    public static LazyList<T, TBase> Create<T, [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicParameterlessConstructor)] TBase>(LazyList<TBase> list, int skip = -1, int take = -1)
        where T : class, TBase
        where TBase : class
    {
        return new LazyList<T, TBase>(list, skip, take);
    }
}
