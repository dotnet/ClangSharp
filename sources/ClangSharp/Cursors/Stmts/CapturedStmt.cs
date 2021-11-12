// Copyright (c) .NET Foundation and Contributors. All Rights Reserved. Licensed under the MIT License (MIT). See License.md in the repository root for more information.

using System;
using System.Collections.Generic;
using System.Linq;
using ClangSharp.Interop;

namespace ClangSharp
{
    public sealed partial class CapturedStmt : Stmt
    {
        private readonly Lazy<CapturedDecl> _capturedDecl;
        private readonly Lazy<RecordDecl> _capturedRecordDecl;
        private readonly Lazy<Stmt> _captureStmt;
        private readonly Lazy<IReadOnlyList<Capture>> _captures;
        private readonly Lazy<IReadOnlyList<Expr>> _captureInits;

        internal CapturedStmt(CXCursor handle) : base(handle, CXCursorKind.CXCursor_UnexposedStmt, CX_StmtClass.CX_StmtClass_CapturedStmt)
        {
            _capturedDecl = new Lazy<CapturedDecl>(() => TranslationUnit.GetOrCreate<CapturedDecl>(Handle.CapturedDecl));
            _capturedRecordDecl = new Lazy<RecordDecl>(() => TranslationUnit.GetOrCreate<RecordDecl>(Handle.CapturedRecordDecl));
            _captureStmt = new Lazy<Stmt>(() => TranslationUnit.GetOrCreate<Stmt>(Handle.CapturedStmt));

            _captures = new Lazy<IReadOnlyList<Capture>>(() => {
                var numCaptures = Handle.NumCaptures;
                var captures = new List<Capture>(numCaptures);

                for (var i = 0; i < numCaptures; i++)
                {
                    var capture = new Capture(this, unchecked((uint)i));
                    captures.Add(capture);
                }

                return captures;
            });

            _captureInits = new Lazy<IReadOnlyList<Expr>>(() => Children.Cast<Expr>().ToList());
        }

        public CapturedDecl CapturedDecl => _capturedDecl.Value;

        public RecordDecl CapturedRecordDecl => _capturedRecordDecl.Value;

        public CX_CapturedRegionKind CapturedRegionKind => Handle.CapturedRegionKind;

        public Stmt CaptureStmt => _captureStmt.Value;

        public IReadOnlyList<Capture> Captures => _captures.Value;

        public uint CaptureSize => unchecked((uint)Handle.NumCaptures);

        public IReadOnlyList<Expr> CaptureInits => _captureInits.Value;

        public bool CapturesVariable(VarDecl var)
        {
            foreach (var i in Captures)
            {
                if (!i.CapturesVariable && !i.CapturesVariableByCopy)
                {
                    continue;
                }

                if (i.CapturedVar.CanonicalDecl == var.CanonicalDecl)
                {
                    return true;
                }
            }
            return false;
        }
    }
}
