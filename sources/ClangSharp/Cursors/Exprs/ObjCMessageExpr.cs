// Copyright (c) .NET Foundation and Contributors. All Rights Reserved. Licensed under the MIT License (MIT). See License.md in the repository root for more information.

using System.Collections.Generic;
using ClangSharp.Interop;
using static ClangSharp.Interop.CX_StmtClass;
using static ClangSharp.Interop.CXCursorKind;

namespace ClangSharp;

public sealed class ObjCMessageExpr : Expr
{
    private readonly LazyList<Expr> _args;
    private ValueLazy<ObjCMessageExpr, Type> _classReceiver;
    private ValueLazy<ObjCMessageExpr, Expr> _instanceReceiver;
    private ValueLazy<ObjCMessageExpr, ObjCMethodDecl> _methodDecl;
    private ValueLazy<ObjCMessageExpr, Type> _receiverType;
    private ValueLazy<ObjCMessageExpr, Type> _superType;

    internal unsafe ObjCMessageExpr(CXCursor handle) : base(handle, CXCursor_ObjCMessageExpr, CX_StmtClass_ObjCMessageExpr)
    {
        _args = LazyList.Create<Expr>(this, Handle.NumArguments, &ArgsFactory);
        _classReceiver = new ValueLazy<ObjCMessageExpr, Type>(&ClassReceiverFactory);
        _instanceReceiver = new ValueLazy<ObjCMessageExpr, Expr>(&InstanceReceiverFactory);
        _methodDecl = new ValueLazy<ObjCMessageExpr, ObjCMethodDecl>(&MethodDeclFactory);
        _receiverType = new ValueLazy<ObjCMessageExpr, Type>(&ReceiverTypeFactory);
        _superType = new ValueLazy<ObjCMessageExpr, Type>(&SuperTypeFactory);
    }

    public IReadOnlyList<Expr> Args => _args;

    public Type ClassReceiver => _classReceiver.GetValue(this);

    public bool IsImplicit => Handle.IsImplicit;

    public Expr InstanceReceiver => _instanceReceiver.GetValue(this);

    public Type ReceiverType => _receiverType.GetValue(this);

    public Type SuperType => _superType.GetValue(this);

    public ObjCMethodDecl MethodDecl => _methodDecl.GetValue(this);

    public uint NumArgs => unchecked((uint)Handle.NumArguments);

    private static unsafe Type SuperTypeFactory(ObjCMessageExpr self) => self.TranslationUnit.GetOrCreate<Type>(self.Handle.ThisObjectType);

    private static unsafe Type ReceiverTypeFactory(ObjCMessageExpr self) => self.TranslationUnit.GetOrCreate<Type>(self.Handle.ReceiverType);

    private static unsafe ObjCMethodDecl MethodDeclFactory(ObjCMessageExpr self) => self.TranslationUnit.GetOrCreate<ObjCMethodDecl>(self.Handle.Referenced);

    private static unsafe Expr InstanceReceiverFactory(ObjCMessageExpr self) => self.TranslationUnit.GetOrCreate<Expr>(self.Handle.GetExpr(0));

    private static unsafe Type ClassReceiverFactory(ObjCMessageExpr self) => self.TranslationUnit.GetOrCreate<Type>(self.Handle.TypeOperand);

    private static unsafe Expr ArgsFactory(object self, int i)
    {
        var @this = (ObjCMessageExpr)self;
        return @this.TranslationUnit.GetOrCreate<Expr>(@this.Handle.GetArgument(unchecked((uint)i)));
    }
}
