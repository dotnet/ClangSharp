// Copyright (c) .NET Foundation and Contributors. All Rights Reserved. Licensed under the MIT License (MIT). See License.md in the repository root for more information.

using ClangSharp.Interop;
using static ClangSharp.Interop.CXCursorKind;
using static ClangSharp.Interop.CX_StmtClass;

namespace ClangSharp;

public sealed class ObjCPropertyRefExpr : Expr
{
    private ValueLazy<Expr> _base;
    private ValueLazy<ObjCInterfaceDecl> _classReceiver;
    private ValueLazy<ObjCPropertyDecl> _explicitProperty;
    private ValueLazy<ObjCMethodDecl> _implicitPropertyGetter;
    private ValueLazy<ObjCMethodDecl> _implicitPropertySetter;
    private ValueLazy<Type> _superReceiverType;

    internal ObjCPropertyRefExpr(CXCursor handle) : base(handle, CXCursor_MemberRefExpr, CX_StmtClass_ObjCPropertyRefExpr)
    {
        _base = new ValueLazy<Expr>(() => TranslationUnit.GetOrCreate<Expr>(Handle.GetExpr(0)));
        _classReceiver = new ValueLazy<ObjCInterfaceDecl>(() => TranslationUnit.GetOrCreate<ObjCInterfaceDecl>(Handle.GetDecl(0)));
        _explicitProperty = new ValueLazy<ObjCPropertyDecl>(() => TranslationUnit.GetOrCreate<ObjCPropertyDecl>(Handle.GetDecl(1)));
        _implicitPropertyGetter = new ValueLazy<ObjCMethodDecl>(() => TranslationUnit.GetOrCreate<ObjCMethodDecl>(Handle.GetDecl(2)));
        _implicitPropertySetter = new ValueLazy<ObjCMethodDecl>(() => TranslationUnit.GetOrCreate<ObjCMethodDecl>(Handle.GetDecl(3)));
        _superReceiverType = new ValueLazy<Type>(() => TranslationUnit.GetOrCreate<Type>(Handle.TypeOperand));
    }


    public Expr Base => _base.Value;

    public ObjCInterfaceDecl ClassReceiver => _classReceiver.Value;

    public ObjCPropertyDecl ExplicitProperty => _explicitProperty.Value;

    public bool IsExplicitProperty => !IsImplicitProperty;

    public bool IsImplicitProperty => Handle.IsImplicit;

    public ObjCMethodDecl ImplicitPropertyGetter => _implicitPropertyGetter.Value;

    public ObjCMethodDecl ImplicitPropertySetter => _implicitPropertySetter.Value;

    public Type SuperReceiverType => _superReceiverType.Value;
}
