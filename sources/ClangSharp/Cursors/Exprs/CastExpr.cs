// Copyright (c) .NET Foundation and Contributors. All Rights Reserved. Licensed under the MIT License (MIT). See License.md in the repository root for more information.

using System;
using System.Collections.Generic;
using System.Diagnostics;
using ClangSharp.Interop;

namespace ClangSharp;

public class CastExpr : Expr
{
    private readonly Lazy<IReadOnlyList<CXXBaseSpecifier>> _path;
    private readonly Lazy<FieldDecl> _targetUnionField;

    private protected CastExpr(CXCursor handle, CXCursorKind expectedCursorKind, CX_StmtClass expectedStmtClass) : base(handle, expectedCursorKind, expectedStmtClass)
    {
        if (handle.StmtClass is > CX_StmtClass.CX_StmtClass_LastCastExpr or < CX_StmtClass.CX_StmtClass_FirstCastExpr)
        {
            throw new ArgumentOutOfRangeException(nameof(handle));
        }

        Debug.Assert(NumChildren is 1);

        _path = new Lazy<IReadOnlyList<CXXBaseSpecifier>>(() => {
            var pathSize = Handle.NumArguments;
            var path = new List<CXXBaseSpecifier>(pathSize);

            for (var i = 0; i < pathSize; i++)
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

    public NamedDecl? ConversionFunction
    {
        get
        {
            Expr subExpr;

            for (var e = this; e is not null; e = subExpr as ImplicitCastExpr)
            {
                subExpr = SkipImplicitTemporary(e.SubExpr);

                if (e.CastKind == CX_CastKind.CX_CK_ConstructorConversion)
                {
                    return ((CXXConstructExpr)subExpr).Constructor;
                }

                if (e.CastKind == CX_CastKind.CX_CK_UserDefinedConversion)
                {
                    if (subExpr is CXXMemberCallExpr mce)
                    {
                        return mce.MethodDecl;
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
            Expr? subExpr;
            var e = this;

            do
            {
                subExpr = SkipImplicitTemporary(e.SubExpr);

                // Conversions by constructor and conversion functions have a subexpression describing the call; strip it off.
                if (e.CastKind == CX_CastKind.CX_CK_ConstructorConversion)
                {
                    subExpr = SkipImplicitTemporary(((CXXConstructExpr)subExpr).Args[0]);
                }
                else if (e.CastKind == CX_CastKind.CX_CK_UserDefinedConversion)
                {
                    Debug.Assert(subExpr is CXXMemberCallExpr or BlockExpr, "Unexpected SubExpr for CK_UserDefinedConversion.");

                    if (subExpr is CXXMemberCallExpr mce)
                    {
                        subExpr = mce.ImplicitObjectArgument;
                        Debug.Assert(subExpr is not null);
                    }
                }

                // If the subexpression we're left with is an implicit cast, look
                // through that, too.
            } while ((e = subExpr as ImplicitCastExpr) is not null);

            Debug.Assert(subExpr is not null);
            return subExpr!;
        }
    }

    public FieldDecl TargetUnionField => _targetUnionField.Value;
}
