// Copyright (c) .NET Foundation and Contributors. All Rights Reserved. Licensed under the MIT License (MIT). See License.md in the repository root for more information.

using System.Collections.Generic;
using ClangSharp.Interop;
using static ClangSharp.Interop.CX_StmtClass;
using static ClangSharp.Interop.CXCursorKind;

namespace ClangSharp;

public sealed class ObjCMessageExpr : Expr
{
    private readonly LazyList<Expr> _args;
    private readonly ValueLazy<Type> _classReceiver;
    private readonly ValueLazy<Expr> _instanceReceiver;
    private readonly ValueLazy<ObjCMethodDecl> _methodDecl;
    private readonly ValueLazy<Type> _receiverType;
    private readonly ValueLazy<Type> _superType;

    internal ObjCMessageExpr(CXCursor handle) : base(handle, CXCursor_ObjCMessageExpr, CX_StmtClass_ObjCMessageExpr)
    {
        _args = LazyList.Create<Expr>(Handle.NumArguments, (i) => TranslationUnit.GetOrCreate<Expr>(Handle.GetArgument(unchecked((uint)i))));
        _classReceiver = new ValueLazy<Type>(() => TranslationUnit.GetOrCreate<Type>(Handle.TypeOperand));
        _instanceReceiver = new ValueLazy<Expr>(() => TranslationUnit.GetOrCreate<Expr>(Handle.GetExpr(0)));
        _methodDecl = new ValueLazy<ObjCMethodDecl>(() => TranslationUnit.GetOrCreate<ObjCMethodDecl>(Handle.Referenced));
        _receiverType = new ValueLazy<Type>(() => TranslationUnit.GetOrCreate<Type>(Handle.ReceiverType));
        _superType = new ValueLazy<Type>(() => TranslationUnit.GetOrCreate<Type>(Handle.ThisObjectType));
    }

    public IReadOnlyList<Expr> Args => _args;

    public Type ClassReceiver => _classReceiver.Value;

    public bool IsImplicit => Handle.IsImplicit;

    public Expr InstanceReceiver => _instanceReceiver.Value;

    public Type ReceiverType => _receiverType.Value;

    public Type SuperType => _superType.Value;

    public ObjCMethodDecl MethodDecl => _methodDecl.Value;

    public uint NumArgs => unchecked((uint)Handle.NumArguments);
}
