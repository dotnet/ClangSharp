// Copyright (c) .NET Foundation and Contributors. All Rights Reserved. Licensed under the MIT License (MIT). See License.md in the repository root for more information.

namespace ClangSharp;

public partial class BlockDecl
{
    public sealed class Capture
    {
        private readonly Decl _parentDecl;
        private readonly uint _index;
        private ValueLazy<Capture, Expr> _copyExpr;
        private ValueLazy<Capture, VarDecl> _variable;

        internal unsafe Capture(Decl parentDecl, uint index)
        {
            _parentDecl = parentDecl;
            _index = index;

            _copyExpr = new ValueLazy<Capture, Expr>(&CopyExprFactory);
            _variable = new ValueLazy<Capture, VarDecl>(&VariableFactory);
        }

        public Expr CopyExpr => _copyExpr.GetValue(this);

        public bool HasCopyExpr => _parentDecl.Handle.GetCaptureHasCopyExpr(_index);

        public bool IsByRef => _parentDecl.Handle.GetCaptureIsByRef(_index);

        public bool IsEscapingByRef => _parentDecl.Handle.GetCaptureIsEscapingByRef(_index);

        public bool IsNested => _parentDecl.Handle.GetCaptureIsNested(_index);

        public bool IsNonEscapingByRef => _parentDecl.Handle.GetCaptureIsNonEscapingByRef(_index);

        public VarDecl Variable => _variable.GetValue(this);
    
    private static unsafe VarDecl VariableFactory(Capture self) => self._parentDecl.TranslationUnit.GetOrCreate<VarDecl>(self._parentDecl.Handle.GetCaptureVariable(self._index));

    private static unsafe Expr CopyExprFactory(Capture self) => self._parentDecl.TranslationUnit.GetOrCreate<Expr>(self._parentDecl.Handle.GetCaptureCopyExpr(self._index));
}
}
