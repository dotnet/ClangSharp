// Copyright (c) .NET Foundation and Contributors. All Rights Reserved. Licensed under the MIT License (MIT). See License.md in the repository root for more information.

using System.Diagnostics;
using ClangSharp.Interop;
using static ClangSharp.Interop.CXCursorKind;
using static ClangSharp.Interop.CX_StmtClass;

namespace ClangSharp;

public sealed class CXXDeleteExpr : Expr
{
    private ValueLazy<CXXDeleteExpr, Type> _destroyedType;
    private ValueLazy<CXXDeleteExpr, FunctionDecl> _operatorDelete;

    internal unsafe CXXDeleteExpr(CXCursor handle) : base(handle, CXCursor_CXXDeleteExpr, CX_StmtClass_CXXDeleteExpr)
    {
        Debug.Assert(NumChildren is 1);

        _destroyedType = new ValueLazy<CXXDeleteExpr, Type>(&DestroyedTypeFactory);
        _operatorDelete = new ValueLazy<CXXDeleteExpr, FunctionDecl>(&OperatorDeleteFactory);
    }

    public Expr Argument => (Expr)Children[0];

    public Type DestroyedType => _destroyedType.GetValue(this);

    public bool DoesUsualArrayDeleteWantSize => Handle.DoesUsualArrayDeleteWantSize;

    public bool IsArrayForm => Handle.IsArrayForm;

    public bool IsArrayFormAsWritten => Handle.IsArrayFormAsWritten;

    public bool IsGlobalDelete => Handle.IsGlobal;

    public FunctionDecl OperatorDelete => _operatorDelete.GetValue(this);

    private static unsafe FunctionDecl OperatorDeleteFactory(CXXDeleteExpr self) => self.TranslationUnit.GetOrCreate<FunctionDecl>(self.Handle.Referenced);

    private static unsafe Type DestroyedTypeFactory(CXXDeleteExpr self) => self.TranslationUnit.GetOrCreate<Type>(self.Handle.TypeOperand);
}
