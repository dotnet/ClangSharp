// Copyright (c) .NET Foundation and Contributors. All Rights Reserved. Licensed under the MIT License (MIT). See License.md in the repository root for more information.

using ClangSharp.Interop;
using static ClangSharp.Interop.CXCursorKind;
using static ClangSharp.Interop.CX_StmtClass;

namespace ClangSharp;

public sealed class ObjCPropertyRefExpr : Expr
{
    private ValueLazy<ObjCPropertyRefExpr, Expr> _base;
    private ValueLazy<ObjCPropertyRefExpr, ObjCInterfaceDecl> _classReceiver;
    private ValueLazy<ObjCPropertyRefExpr, ObjCPropertyDecl> _explicitProperty;
    private ValueLazy<ObjCPropertyRefExpr, ObjCMethodDecl> _implicitPropertyGetter;
    private ValueLazy<ObjCPropertyRefExpr, ObjCMethodDecl> _implicitPropertySetter;
    private ValueLazy<ObjCPropertyRefExpr, Type> _superReceiverType;

    internal unsafe ObjCPropertyRefExpr(CXCursor handle) : base(handle, CXCursor_MemberRefExpr, CX_StmtClass_ObjCPropertyRefExpr)
    {
        _base = new ValueLazy<ObjCPropertyRefExpr, Expr>(&BaseFactory);
        _classReceiver = new ValueLazy<ObjCPropertyRefExpr, ObjCInterfaceDecl>(&ClassReceiverFactory);
        _explicitProperty = new ValueLazy<ObjCPropertyRefExpr, ObjCPropertyDecl>(&ExplicitPropertyFactory);
        _implicitPropertyGetter = new ValueLazy<ObjCPropertyRefExpr, ObjCMethodDecl>(&ImplicitPropertyGetterFactory);
        _implicitPropertySetter = new ValueLazy<ObjCPropertyRefExpr, ObjCMethodDecl>(&ImplicitPropertySetterFactory);
        _superReceiverType = new ValueLazy<ObjCPropertyRefExpr, Type>(&SuperReceiverTypeFactory);
    }


    public Expr Base => _base.GetValue(this);

    public ObjCInterfaceDecl ClassReceiver => _classReceiver.GetValue(this);

    public ObjCPropertyDecl ExplicitProperty => _explicitProperty.GetValue(this);

    public bool IsExplicitProperty => !IsImplicitProperty;

    public bool IsImplicitProperty => Handle.IsImplicit;

    public bool IsMessagingGetter => Handle.IsMessagingGetter;

    public bool IsMessagingSetter => Handle.IsMessagingSetter;

    public ObjCMethodDecl ImplicitPropertyGetter => _implicitPropertyGetter.GetValue(this);

    public ObjCMethodDecl ImplicitPropertySetter => _implicitPropertySetter.GetValue(this);

    public CX_ObjCPropertyRefReceiverKind ReceiverKind => Handle.ObjCPropertyRefReceiverKind;

    public Type SuperReceiverType => _superReceiverType.GetValue(this);

    private static unsafe Type SuperReceiverTypeFactory(ObjCPropertyRefExpr self) => self.TranslationUnit.GetOrCreate<Type>(self.Handle.TypeOperand);

    private static unsafe ObjCMethodDecl ImplicitPropertySetterFactory(ObjCPropertyRefExpr self) => self.TranslationUnit.GetOrCreate<ObjCMethodDecl>(self.Handle.GetDecl(3));

    private static unsafe ObjCMethodDecl ImplicitPropertyGetterFactory(ObjCPropertyRefExpr self) => self.TranslationUnit.GetOrCreate<ObjCMethodDecl>(self.Handle.GetDecl(2));

    private static unsafe ObjCPropertyDecl ExplicitPropertyFactory(ObjCPropertyRefExpr self) => self.TranslationUnit.GetOrCreate<ObjCPropertyDecl>(self.Handle.GetDecl(1));

    private static unsafe ObjCInterfaceDecl ClassReceiverFactory(ObjCPropertyRefExpr self) => self.TranslationUnit.GetOrCreate<ObjCInterfaceDecl>(self.Handle.GetDecl(0));

    private static unsafe Expr BaseFactory(ObjCPropertyRefExpr self) => self.TranslationUnit.GetOrCreate<Expr>(self.Handle.GetExpr(0));
}
