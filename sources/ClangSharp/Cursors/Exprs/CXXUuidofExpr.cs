// Copyright (c) .NET Foundation and Contributors. All Rights Reserved. Licensed under the MIT License (MIT). See License.md in the repository root for more information.

using System.Diagnostics;
using System.Linq;
using ClangSharp.Interop;
using static ClangSharp.Interop.CXCursorKind;
using static ClangSharp.Interop.CX_StmtClass;

namespace ClangSharp;

public sealed class CXXUuidofExpr : Expr
{
    private ValueLazy<CXXUuidofExpr, Type> _typeOperand;
    private ValueLazy<CXXUuidofExpr, MSGuidDecl> _guidDecl;

    internal unsafe CXXUuidofExpr(CXCursor handle) : base(handle, CXCursor_UnexposedExpr, CX_StmtClass_CXXUuidofExpr)
    {
        Debug.Assert(NumChildren is 0 or 1);

        _typeOperand = new ValueLazy<CXXUuidofExpr, Type>(&TypeOperandFactory);
        _guidDecl = new ValueLazy<CXXUuidofExpr, MSGuidDecl>(&GuidDeclFactory);
    }

    public Expr? ExprOperand => (Expr?)Children.SingleOrDefault();

    public MSGuidDecl GuidDecl => _guidDecl.GetValue(this);

    public bool IsTypeOperand => NumChildren is 0;

    public Type TypeOperand => _typeOperand.GetValue(this);

    private static unsafe MSGuidDecl GuidDeclFactory(CXXUuidofExpr self) => self.TranslationUnit.GetOrCreate<MSGuidDecl>(self.Handle.Referenced);

    private static unsafe Type TypeOperandFactory(CXXUuidofExpr self) => self.TranslationUnit.GetOrCreate<Type>(self.Handle.TypeOperand);
}
