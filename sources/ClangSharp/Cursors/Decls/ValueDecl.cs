// Copyright (c) .NET Foundation and Contributors. All Rights Reserved. Licensed under the MIT License (MIT). See License.md in the repository root for more information.

using System;
using ClangSharp.Interop;

namespace ClangSharp;

public class ValueDecl : NamedDecl
{
    private readonly Lazy<Type> _type;

    private protected ValueDecl(CXCursor handle, CXCursorKind expectedCursorKind, CX_DeclKind expectedDeclKind) : base(handle, expectedCursorKind, expectedDeclKind)
    {
        if (handle.DeclKind is > CX_DeclKind.CX_DeclKind_LastValue or < CX_DeclKind.CX_DeclKind_FirstValue)
        {
            throw new ArgumentOutOfRangeException(nameof(handle));
        }

        _type = new Lazy<Type>(() => TranslationUnit.GetOrCreate<Type>(Handle.Type));
    }

    public Type Type => _type.Value;
}
