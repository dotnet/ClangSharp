// Copyright (c) .NET Foundation and Contributors. All Rights Reserved. Licensed under the MIT License (MIT). See License.md in the repository root for more information.

using System;

namespace ClangSharp
{
    public partial class BlockDecl
    {
        public sealed class Capture
        {
            private readonly Decl _parentDecl;
            private readonly uint _index;
            private readonly Lazy<Expr> _copyExpr;
            private readonly Lazy<VarDecl> _variable;

            internal Capture(Decl parentDecl, uint index)
            {
                _parentDecl = parentDecl;
                _index = index;

                _copyExpr = new Lazy<Expr>(() => _parentDecl.TranslationUnit.GetOrCreate<Expr>(_parentDecl.Handle.GetCaptureCopyExpr(_index)));
                _variable = new Lazy<VarDecl>(() => _parentDecl.TranslationUnit.GetOrCreate<VarDecl>(_parentDecl.Handle.GetCaptureVariable(_index)));
            }

            public Expr CopyExpr => _copyExpr.Value;

            public bool HasCopyExpr => _parentDecl.Handle.GetCaptureHasCopyExpr(_index);

            public bool IsByRef => _parentDecl.Handle.GetCaptureIsByRef(_index);

            public bool IsEscapingByRef => _parentDecl.Handle.GetCaptureIsEscapingByRef(_index);

            public bool IsNested => _parentDecl.Handle.GetCaptureIsNested(_index);

            public bool IsNonEscapingByRef => _parentDecl.Handle.GetCaptureIsNonEscapingByRef(_index);

            public VarDecl Variable => _variable.Value;
        }
    }
}
