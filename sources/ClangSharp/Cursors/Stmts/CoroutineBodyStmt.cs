// Copyright (c) .NET Foundation and Contributors. All Rights Reserved. Licensed under the MIT License (MIT). See License.md in the repository root for more information.

using ClangSharp.Interop;
using static ClangSharp.Interop.CXCursorKind;
using static ClangSharp.Interop.CX_StmtClass;

namespace ClangSharp;

public sealed class CoroutineBodyStmt : Stmt
{
    private ValueLazy<CoroutineBodyStmt, Expr> _allocate;
    private ValueLazy<CoroutineBodyStmt, CompoundStmt> _body;
    private ValueLazy<CoroutineBodyStmt, Expr> _deallocate;
    private ValueLazy<CoroutineBodyStmt, Stmt> _exceptionHandler;
    private ValueLazy<CoroutineBodyStmt, Stmt> _fallthroughHandler;
    private ValueLazy<CoroutineBodyStmt, Stmt> _finalSuspendStmt;
    private ValueLazy<CoroutineBodyStmt, Stmt> _initSuspendStmt;
    private ValueLazy<CoroutineBodyStmt, VarDecl> _promiseDecl;
    private ValueLazy<CoroutineBodyStmt, Stmt> _resultDecl;
    private ValueLazy<CoroutineBodyStmt, Stmt> _returnStmt;
    private ValueLazy<CoroutineBodyStmt, Stmt> _returnStmtOnAllocFailure;
    private ValueLazy<CoroutineBodyStmt, Expr> _returnValue;
    private ValueLazy<CoroutineBodyStmt, Expr> _returnValueInit;

    internal unsafe CoroutineBodyStmt(CXCursor handle) : base(handle, CXCursor_UnexposedStmt, CX_StmtClass_CoroutineBodyStmt)
    {
        _allocate = new ValueLazy<CoroutineBodyStmt, Expr>(&AllocateFactory);
        _body = new ValueLazy<CoroutineBodyStmt, CompoundStmt>(&BodyFactory);
        _deallocate = new ValueLazy<CoroutineBodyStmt, Expr>(&DeallocateFactory);
        _exceptionHandler = new ValueLazy<CoroutineBodyStmt, Stmt>(&ExceptionHandlerFactory);
        _fallthroughHandler = new ValueLazy<CoroutineBodyStmt, Stmt>(&FallthroughHandlerFactory);
        _finalSuspendStmt = new ValueLazy<CoroutineBodyStmt, Stmt>(&FinalSuspendStmtFactory);
        _initSuspendStmt = new ValueLazy<CoroutineBodyStmt, Stmt>(&InitSuspendStmtFactory);
        _promiseDecl = new ValueLazy<CoroutineBodyStmt, VarDecl>(&PromiseDeclFactory);
        _resultDecl = new ValueLazy<CoroutineBodyStmt, Stmt>(&ResultDeclFactory);
        _returnStmt = new ValueLazy<CoroutineBodyStmt, Stmt>(&ReturnStmtFactory);
        _returnStmtOnAllocFailure = new ValueLazy<CoroutineBodyStmt, Stmt>(&ReturnStmtOnAllocFailureFactory);
        _returnValue = new ValueLazy<CoroutineBodyStmt, Expr>(&ReturnValueFactory);
        _returnValueInit = new ValueLazy<CoroutineBodyStmt, Expr>(&ReturnValueInitFactory);
    }

    public Expr Allocate => _allocate.GetValue(this);

    public CompoundStmt Body => _body.GetValue(this);

    public Expr Deallocate => _deallocate.GetValue(this);

    public Stmt ExceptionHandler => _exceptionHandler.GetValue(this);

    public Stmt FallthroughHandler => _fallthroughHandler.GetValue(this);

    public Stmt FinalSuspendStmt => _finalSuspendStmt.GetValue(this);

    public bool HasDependentPromiseType => Handle.HasDependentPromiseType;

    public Stmt InitSuspendStmt => _initSuspendStmt.GetValue(this);

    public VarDecl PromiseDecl => _promiseDecl.GetValue(this);

    public Stmt ResultDecl => _resultDecl.GetValue(this);

    public Stmt ReturnStmt => _returnStmt.GetValue(this);

    public Stmt ReturnStmtOnAllocFailure => _returnStmtOnAllocFailure.GetValue(this);

    public Expr ReturnValue => _returnValue.GetValue(this);

    public Expr ReturnValueInit => _returnValueInit.GetValue(this);

    private static Expr AllocateFactory(CoroutineBodyStmt self) => self.TranslationUnit.GetOrCreate<Expr>(self.Handle.Allocate);

    private static CompoundStmt BodyFactory(CoroutineBodyStmt self) => self.TranslationUnit.GetOrCreate<CompoundStmt>(self.Handle.Body);

    private static Expr DeallocateFactory(CoroutineBodyStmt self) => self.TranslationUnit.GetOrCreate<Expr>(self.Handle.Deallocate);

    private static Stmt ExceptionHandlerFactory(CoroutineBodyStmt self) => self.TranslationUnit.GetOrCreate<Stmt>(self.Handle.ExceptionHandler);

    private static Stmt FallthroughHandlerFactory(CoroutineBodyStmt self) => self.TranslationUnit.GetOrCreate<Stmt>(self.Handle.FallthroughHandler);

    private static Stmt FinalSuspendStmtFactory(CoroutineBodyStmt self) => self.TranslationUnit.GetOrCreate<Stmt>(self.Handle.FinalSuspendStmt);

    private static Stmt InitSuspendStmtFactory(CoroutineBodyStmt self) => self.TranslationUnit.GetOrCreate<Stmt>(self.Handle.InitSuspendStmt);

    private static VarDecl PromiseDeclFactory(CoroutineBodyStmt self) => self.TranslationUnit.GetOrCreate<VarDecl>(self.Handle.PromiseDecl);

    private static Stmt ResultDeclFactory(CoroutineBodyStmt self) => self.TranslationUnit.GetOrCreate<Stmt>(self.Handle.ResultDecl);

    private static Stmt ReturnStmtFactory(CoroutineBodyStmt self) => self.TranslationUnit.GetOrCreate<Stmt>(self.Handle.ReturnStmt);

    private static Stmt ReturnStmtOnAllocFailureFactory(CoroutineBodyStmt self) => self.TranslationUnit.GetOrCreate<Stmt>(self.Handle.ReturnStmtOnAllocFailure);

    private static Expr ReturnValueFactory(CoroutineBodyStmt self) => self.TranslationUnit.GetOrCreate<Expr>(self.Handle.ReturnValue);

    private static Expr ReturnValueInitFactory(CoroutineBodyStmt self) => self.TranslationUnit.GetOrCreate<Expr>(self.Handle.ReturnValueInit);
}
