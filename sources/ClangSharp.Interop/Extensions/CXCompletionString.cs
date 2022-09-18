// Copyright (c) .NET Foundation and Contributors. All Rights Reserved. Licensed under the MIT License (MIT). See License.md in the repository root for more information.

using System;

namespace ClangSharp.Interop;

public unsafe partial struct CXCompletionString : IEquatable<CXCompletionString>
{
    public CXCompletionString(IntPtr handle)
    {
        Handle = handle;
    }

    public CXAvailabilityKind Availability => clang.getCompletionAvailability(this);

    public CXString BriefComment => clang.getCompletionBriefComment(this);

    public IntPtr Handle { get; set; }

    public uint NumAnnotations => clang.getCompletionNumAnnotations(this);

    public uint NumChunks => clang.getNumCompletionChunks(this);

    public uint Priority => clang.getCompletionPriority(this);

    public static explicit operator CXCompletionString(void* value) => new CXCompletionString((IntPtr)value);

    public static implicit operator void*(CXCompletionString value) => (void*)value.Handle;

    public static bool operator ==(CXCompletionString left, CXCompletionString right) => left.Handle == right.Handle;

    public static bool operator !=(CXCompletionString left, CXCompletionString right) => left.Handle != right.Handle;

    public override bool Equals(object? obj) => (obj is CXCompletionString other) && Equals(other);

    public bool Equals(CXCompletionString other) => this == other;

    public CXString GetAnnotation(uint index) => clang.getCompletionAnnotation(this, index);

    public CXCompletionString GetChunkCompletionString(uint index) => (CXCompletionString)clang.getCompletionChunkCompletionString(this, index);

    public CXCompletionChunkKind GetChunkKind(uint index) => clang.getCompletionChunkKind(this, index);

    public CXString GetChunkText(uint index) => clang.getCompletionChunkText(this, index);

    public override int GetHashCode() => Handle.GetHashCode();

    public CXString GetParent(out CXCursorKind kind)
    {
        fixed (CXCursorKind* pKind = &kind)
        {
            return clang.getCompletionParent(this, pKind);
        }
    }
}
