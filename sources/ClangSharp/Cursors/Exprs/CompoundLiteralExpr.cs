// Copyright (c) .NET Foundation and Contributors. All Rights Reserved. Licensed under the MIT License (MIT). See License.md in the repository root for more information.

using System;
using System.Diagnostics;
using ClangSharp.Interop;
using static ClangSharp.Interop.CXCursorKind;
using static ClangSharp.Interop.CX_StmtClass;

namespace ClangSharp;

public sealed class CompoundLiteralExpr : Expr
{
    private readonly Lazy<Type> _typeSourceinfoType;

    internal CompoundLiteralExpr(CXCursor handle) : base(handle, CXCursor_CompoundLiteralExpr, CX_StmtClass_CompoundLiteralExpr)
    {
        Debug.Assert(NumChildren is 1);
        _typeSourceinfoType = new Lazy<Type>(() => TranslationUnit.GetOrCreate<Type>(Handle.TypeOperand));
    }

    public bool IsFileScope => Handle.IsFileScope;

    public Expr Initializer => (Expr)Children[0];

    public Type TypeSourceInfoType => _typeSourceinfoType.Value;
}
