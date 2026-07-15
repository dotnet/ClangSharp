// Copyright (c) .NET Foundation and Contributors. All Rights Reserved. Licensed under the MIT License (MIT). See License.md in the repository root for more information.

using ClangSharp.Interop;
using static ClangSharp.Interop.CXCursorKind;
using static ClangSharp.Interop.CX_StmtClass;

namespace ClangSharp;

public sealed class LambdaExpr : Expr
{
    private ValueLazy<LambdaExpr, CXXMethodDecl> _callOperator;

    internal unsafe LambdaExpr(CXCursor handle) : base(handle, CXCursor_LambdaExpr, CX_StmtClass_LambdaExpr)
    {
        _callOperator = new ValueLazy<LambdaExpr, CXXMethodDecl>(&CallOperatorFactory);
    }

    public CXXMethodDecl CallOperator => _callOperator.GetValue(this);

    public bool IsMutable => !CallOperator.IsConst;

    private static unsafe CXXMethodDecl CallOperatorFactory(LambdaExpr self) => self.TranslationUnit.GetOrCreate<CXXMethodDecl>(self.Type.AsCXXRecordDecl!.Handle.LambdaCallOperator);
}
