// Copyright (c) .NET Foundation and Contributors. All Rights Reserved. Licensed under the MIT License (MIT). See License.md in the repository root for more information.

using System.Diagnostics;
using ClangSharp.Interop;
using static ClangSharp.Interop.CXCursorKind;
using static ClangSharp.Interop.CX_StmtClass;

namespace ClangSharp;

public sealed class CXXDeleteExpr : Expr
{
    private readonly ValueLazy<Type> _destroyedType;
    private readonly ValueLazy<FunctionDecl> _operatorDelete;

    internal CXXDeleteExpr(CXCursor handle) : base(handle, CXCursor_CXXDeleteExpr, CX_StmtClass_CXXDeleteExpr)
    {
        Debug.Assert(NumChildren is 1);

        _destroyedType = new ValueLazy<Type>(() => TranslationUnit.GetOrCreate<Type>(Handle.TypeOperand));
        _operatorDelete = new ValueLazy<FunctionDecl>(() => TranslationUnit.GetOrCreate<FunctionDecl>(Handle.Referenced));
    }

    public Expr Argument => (Expr)Children[0];

    public Type DestroyedType => _destroyedType.Value;

    public bool DoesUsualArrayDeleteWantSize => Handle.DoesUsualArrayDeleteWantSize;

    public bool IsArrayForm => Handle.IsArrayForm;

    public bool IsArrayFormAsWritten => Handle.IsArrayFormAsWritten;

    public bool IsGlobalDelete => Handle.IsGlobal;

    public FunctionDecl OperatorDelete => _operatorDelete.Value;
}
