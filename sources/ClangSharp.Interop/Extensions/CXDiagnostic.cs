// Copyright (c) .NET Foundation and Contributors. All Rights Reserved. Licensed under the MIT License (MIT). See License.md in the repository root for more information.

using System;

namespace ClangSharp.Interop;

public unsafe partial struct CXDiagnostic(IntPtr handle) : IDisposable, IEquatable<CXDiagnostic>
{
    public static CXDiagnosticDisplayOptions DefaultDisplayOptions => (CXDiagnosticDisplayOptions)clang.defaultDiagnosticDisplayOptions();

    public readonly uint Category => clang.getDiagnosticCategory(this);

    public readonly CXString CategoryText => clang.getDiagnosticCategoryText(this);

    public readonly CXDiagnosticSet ChildDiagnostics => (CXDiagnosticSet)clang.getChildDiagnostics(this);

    public IntPtr Handle { get; set; } = handle;

    public readonly CXSourceLocation Location => clang.getDiagnosticLocation(this);

    public readonly uint NumFixIts => clang.getDiagnosticNumFixIts(this);

    public readonly uint NumRanges => clang.getDiagnosticNumRanges(this);

    public readonly CXDiagnosticSeverity Severity => clang.getDiagnosticSeverity(this);

    public readonly CXString Spelling => clang.getDiagnosticSpelling(this);

    public static explicit operator CXDiagnostic(void* value) => new CXDiagnostic((IntPtr)value);

    public static implicit operator void*(CXDiagnostic value) => (void*)value.Handle;

    public static bool operator ==(CXDiagnostic left, CXDiagnostic right) => left.Handle == right.Handle;

    public static bool operator !=(CXDiagnostic left, CXDiagnostic right) => left.Handle != right.Handle;

    public void Dispose()
    {
        if (Handle != IntPtr.Zero)
        {
            clang.disposeDiagnostic(this);
            Handle = IntPtr.Zero;
        }
    }

    public override readonly bool Equals(object? obj) => (obj is CXDiagnostic other) && Equals(other);

    public readonly bool Equals(CXDiagnostic other) => this == other;

    public readonly CXString Format(CXDiagnosticDisplayOptions options) => clang.formatDiagnostic(this, (uint)options);

    [Obsolete($"Use {nameof(CategoryText)} instead.")]
    public static CXString GetCategoryName(uint category) => clang.getDiagnosticCategoryName(category);

    public readonly CXString GetFixIt(uint fixIt, out CXSourceRange replacementRange)
    {
        fixed (CXSourceRange* pReplacementRange = &replacementRange)
        {
            return clang.getDiagnosticFixIt(this, fixIt, pReplacementRange);
        }
    }

    public override readonly int GetHashCode() => Handle.GetHashCode();

    public readonly CXString GetOption(out CXString disable)
    {
        fixed (CXString* pDisable = &disable)
        {
            return clang.getDiagnosticOption(this, pDisable);
        }
    }

    public readonly CXSourceRange GetRange(uint range) => clang.getDiagnosticRange(this, range);

    public override readonly string ToString() => Spelling.ToString();
}
