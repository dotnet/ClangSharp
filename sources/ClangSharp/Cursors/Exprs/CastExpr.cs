// Copyright (c) Microsoft and Contributors. All rights reserved. Licensed under the University of Illinois/NCSA Open Source License. See LICENSE.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Diagnostics;
using ClangSharp.Interop;

namespace ClangSharp
{
    public class CastExpr : Expr
    {
        private readonly Lazy<IReadOnlyList<CXXBaseSpecifier>> _path;
        private readonly Lazy<FieldDecl> _targetUnionField;

        private protected CastExpr(CXCursor handle, CXCursorKind expectedCursorKind, CX_StmtClass expectedStmtClass) : base(handle, expectedCursorKind, expectedStmtClass)
        {
            if ((CX_StmtClass.CX_StmtClass_LastCastExpr < handle.StmtClass) || (handle.StmtClass < CX_StmtClass.CX_StmtClass_FirstCastExpr))
            {
                throw new ArgumentException(nameof(handle));
            }

            Debug.Assert(NumChildren is 1);

            _path = new Lazy<IReadOnlyList<CXXBaseSpecifier>>(() => {
                var pathSize = Handle.NumArguments;
                var path = new List<CXXBaseSpecifier>(pathSize);

                for (int i = 0; i < pathSize; i++)
                {
                    var item = TranslationUnit.GetOrCreate<CXXBaseSpecifier>(Handle.GetArgument(unchecked((uint)i)));
                    path.Add(item);
                }

                return path;
            });
            _targetUnionField = new Lazy<FieldDecl>(() => TranslationUnit.GetOrCreate<FieldDecl>(Handle.TargetUnionField));
        }

        public CX_CastKind CastKind => Handle.CastKind;

        public string CastKindName => Handle.CastKindSpelling;

        public NamedDecl ConversionFunction
        {
            get
            {
                Expr SubExpr;

                for (CastExpr E = this; E is not null; E = SubExpr as ImplicitCastExpr)
                {
                    SubExpr = SkipImplicitTemporary(E.SubExpr);

                    if (E.CastKind == CX_CastKind.CX_CK_ConstructorConversion)
                    {
                        return ((CXXConstructExpr)SubExpr).Constructor;
                    }

                    if (E.CastKind == CX_CastKind.CX_CK_UserDefinedConversion)
                    {
                        if (SubExpr is CXXMemberCallExpr MCE)
                        {
                            return MCE.MethodDecl;
                        }
                    }
                }

                return null;
            }
        }

        public bool PathEmpty => PathSize == 0;

        public IReadOnlyList<CXXBaseSpecifier> Path => _path.Value;

        public uint PathSize => unchecked((uint)Handle.NumArguments);

        public Expr SubExpr => (Expr)Children[0];

        public Expr SubExprAsWritten
        {
            get
            {
                Expr SubExpr;
                CastExpr E = this;

                do
                {
                    SubExpr = SkipImplicitTemporary(E.SubExpr);

                    // Conversions by constructor and conversion functions have a subexpression describing the call; strip it off.
                    if (E.CastKind == CX_CastKind.CX_CK_ConstructorConversion)
                    {
                        SubExpr = SkipImplicitTemporary(((CXXConstructExpr)SubExpr).Args[0]);
                    }
                    else if (E.CastKind == CX_CastKind.CX_CK_UserDefinedConversion)
                    {
                        Debug.Assert((SubExpr is CXXMemberCallExpr) || (SubExpr is BlockExpr), "Unexpected SubExpr for CK_UserDefinedConversion.");

                        if (SubExpr is CXXMemberCallExpr MCE)
                        {
                            SubExpr = MCE.ImplicitObjectArgument;
                        }
                    }

                    // If the subexpression we're left with is an implicit cast, look
                    // through that, too.
                } while ((E = SubExpr as ImplicitCastExpr) is not null);

                return SubExpr;
            }
        }

        public FieldDecl TargetUnionField => _targetUnionField.Value;
    }
}
