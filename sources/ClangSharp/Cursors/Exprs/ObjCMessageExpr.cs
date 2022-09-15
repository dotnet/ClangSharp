// Copyright (c) .NET Foundation and Contributors. All Rights Reserved. Licensed under the MIT License (MIT). See License.md in the repository root for more information.

using System;
using System.Collections.Generic;
using ClangSharp.Interop;

namespace ClangSharp;

public sealed class ObjCMessageExpr : Expr
{
    private readonly Lazy<IReadOnlyList<Expr>> _args;
    private readonly Lazy<Type> _classReceiver;
    private readonly Lazy<Expr> _instanceReceiver;
    private readonly Lazy<ObjCMethodDecl> _methodDecl;
    private readonly Lazy<Type> _receiverType;
    private readonly Lazy<Type> _superType;

    internal ObjCMessageExpr(CXCursor handle) : base(handle, CXCursorKind.CXCursor_ObjCMessageExpr, CX_StmtClass.CX_StmtClass_ObjCMessageExpr)
    {
        _args = new Lazy<IReadOnlyList<Expr>>(() => {
            var numArgs = Handle.NumArguments;
            var args = new List<Expr>(numArgs);

            for (var i = 0; i < numArgs; i++)
            {
                var arg = TranslationUnit.GetOrCreate<Expr>(Handle.GetArgument(unchecked((uint)i)));
                args.Add(arg);
            }

            return args;
        });

        _classReceiver = new Lazy<Type>(() => TranslationUnit.GetOrCreate<Type>(Handle.TypeOperand));
        _instanceReceiver = new Lazy<Expr>(() => TranslationUnit.GetOrCreate<Expr>(Handle.GetExpr(0)));
        _methodDecl = new Lazy<ObjCMethodDecl>(() => TranslationUnit.GetOrCreate<ObjCMethodDecl>(Handle.Referenced));
        _receiverType = new Lazy<Type>(() => TranslationUnit.GetOrCreate<Type>(Handle.ReceiverType));
        _superType = new Lazy<Type>(() => TranslationUnit.GetOrCreate<Type>(Handle.ThisObjectType));
    }

    public IReadOnlyList<Expr> Args => _args.Value;

    public Type ClassReceiver => _classReceiver.Value;

    public bool IsImplicit => Handle.IsImplicit;

    public Expr InstanceReceiver => _instanceReceiver.Value;

    public Type ReceiverType => _receiverType.Value;

    public Type SuperType => _superType.Value;

    public ObjCMethodDecl MethodDecl => _methodDecl.Value;

    public uint NumArgs => unchecked((uint)Handle.NumArguments);
}
