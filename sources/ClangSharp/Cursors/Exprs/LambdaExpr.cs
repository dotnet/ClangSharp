// Copyright (c) .NET Foundation and Contributors. All Rights Reserved. Licensed under the MIT License (MIT). See License.md in the repository root for more information.

using ClangSharp.Interop;
using static ClangSharp.Interop.CXCursorKind;
using static ClangSharp.Interop.CX_StmtClass;

namespace ClangSharp;

public sealed class LambdaExpr : Expr
{
    private ValueLazy<LambdaExpr, CXXMethodDecl> _callOperator;
    private ValueLazy<LambdaExpr, CXXRecordDecl> _lambdaClass;

    internal unsafe LambdaExpr(CXCursor handle) : base(handle, CXCursor_LambdaExpr, CX_StmtClass_LambdaExpr)
    {
        _callOperator = new ValueLazy<LambdaExpr, CXXMethodDecl>(&CallOperatorFactory);
        _lambdaClass = new ValueLazy<LambdaExpr, CXXRecordDecl>(&LambdaClassFactory);
    }

    public CXXMethodDecl CallOperator => _callOperator.GetValue(this);

    public CX_LambdaCaptureDefault CaptureDefault => LambdaClass.Handle.LambdaCaptureDefault;

    public bool IsCapturelessLambda => LambdaClass.Handle.IsCapturelessLambda;

    public bool IsGenericLambda => LambdaClass.Handle.IsGenericLambda;

    public bool IsMutable => !CallOperator.IsConst;

    public CXXRecordDecl LambdaClass => _lambdaClass.GetValue(this);

    private static unsafe CXXMethodDecl CallOperatorFactory(LambdaExpr self) => self.TranslationUnit.GetOrCreate<CXXMethodDecl>(self.LambdaClass.Handle.LambdaCallOperator);

    private static unsafe CXXRecordDecl LambdaClassFactory(LambdaExpr self) => self.Type.AsCXXRecordDecl!;
}
