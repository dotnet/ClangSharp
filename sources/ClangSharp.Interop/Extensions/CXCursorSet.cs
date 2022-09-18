// Copyright (c) .NET Foundation and Contributors. All Rights Reserved. Licensed under the MIT License (MIT). See License.md in the repository root for more information.

using System;

namespace ClangSharp.Interop;

public unsafe partial struct CXCursorSet : IDisposable, IEquatable<CXCursorSet>
{
    public CXCursorSet(IntPtr handle)
    {
        Handle = handle;
    }

    public IntPtr Handle { get; set; }

    public static implicit operator CXCursorSet(CXCursorSetImpl* value) => new CXCursorSet((IntPtr)value);

    public static implicit operator CXCursorSetImpl*(CXCursorSet value) => (CXCursorSetImpl*)value.Handle;

    public static bool operator ==(CXCursorSet left, CXCursorSet right) => left.Handle == right.Handle;

    public static bool operator !=(CXCursorSet left, CXCursorSet right) => left.Handle != right.Handle;

    public static CXCursorSet Create() => clang.createCXCursorSet();

    public bool Contains(CXCursor cursor) => clang.CXCursorSet_contains(this, cursor) != 0;

    public void Dispose()
    {
        if (Handle != IntPtr.Zero)
        {
            clang.disposeCXCursorSet(this);
            Handle = IntPtr.Zero;
        }
    }

    public override bool Equals(object? obj) => (obj is CXCursorSet other) && Equals(other);

    public bool Equals(CXCursorSet other) => this == other;

    public override int GetHashCode() => Handle.GetHashCode();

    public bool Insert(CXCursor cursor) => clang.CXCursorSet_insert(this, cursor) != 0;
}
