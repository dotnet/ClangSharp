// Copyright (c) .NET Foundation and Contributors. All Rights Reserved. Licensed under the MIT License (MIT). See License.md in the repository root for more information.

using System;
using ClangSharp.Interop;
using static ClangSharp.Interop.CX_DeclKind;

namespace ClangSharp;

public class TypeDecl : NamedDecl
{
    private readonly ValueLazy<Type> _typeForDecl;

    private protected TypeDecl(CXCursor handle, CXCursorKind expectedCursorKind, CX_DeclKind expectedDeclKind) : base(handle, expectedCursorKind, expectedDeclKind)
    {
        if (handle.DeclKind is > CX_DeclKind_LastType or < CX_DeclKind_FirstType)
        {
            throw new ArgumentOutOfRangeException(nameof(handle));
        }

        _typeForDecl = new ValueLazy<Type>(() => TranslationUnit.GetOrCreate<Type>(Handle.Type));
    }

    public Type TypeForDecl => _typeForDecl.Value;
}
