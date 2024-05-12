// Copyright (c) .NET Foundation and Contributors. All Rights Reserved. Licensed under the MIT License (MIT). See License.md in the repository root for more information.

using System;

namespace ClangSharp.Interop;

public unsafe partial struct CXCompletionString(IntPtr handle) : IEquatable<CXCompletionString>
{
    public readonly CXAvailabilityKind Availability => clang.getCompletionAvailability(this);

    public readonly CXString BriefComment => clang.getCompletionBriefComment(this);

    public IntPtr Handle { get; set; } = handle;

    public readonly uint NumAnnotations => clang.getCompletionNumAnnotations(this);

    public readonly uint NumChunks => clang.getNumCompletionChunks(this);

    public readonly uint Priority => clang.getCompletionPriority(this);

    public static explicit operator CXCompletionString(void* value) => new CXCompletionString((IntPtr)value);

    public static implicit operator void*(CXCompletionString value) => (void*)value.Handle;

    public static bool operator ==(CXCompletionString left, CXCompletionString right) => left.Handle == right.Handle;

    public static bool operator !=(CXCompletionString left, CXCompletionString right) => left.Handle != right.Handle;

    public override readonly bool Equals(object? obj) => (obj is CXCompletionString other) && Equals(other);

    public readonly bool Equals(CXCompletionString other) => this == other;

    public readonly CXString GetAnnotation(uint index) => clang.getCompletionAnnotation(this, index);

    public readonly CXCompletionString GetChunkCompletionString(uint index) => (CXCompletionString)clang.getCompletionChunkCompletionString(this, index);

    public readonly CXCompletionChunkKind GetChunkKind(uint index) => clang.getCompletionChunkKind(this, index);

    public readonly CXString GetChunkText(uint index) => clang.getCompletionChunkText(this, index);

    public override readonly int GetHashCode() => Handle.GetHashCode();

    public readonly CXString GetParent(out CXCursorKind kind)
    {
        fixed (CXCursorKind* pKind = &kind)
        {
            return clang.getCompletionParent(this, pKind);
        }
    }
}
