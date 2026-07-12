// Copyright (c) .NET Foundation and Contributors. All Rights Reserved. Licensed under the MIT License (MIT). See License.md in the repository root for more information.

using System.Collections.Generic;
using System.Diagnostics;
using ClangSharp.Interop;
using static ClangSharp.Interop.CXCursorKind;
using static ClangSharp.Interop.CX_StmtClass;

namespace ClangSharp;

public sealed class ObjCDictionaryLiteral : Expr
{
    private ValueLazy<ObjCDictionaryLiteral, ObjCMethodDecl> _dictWithObjectsMethod;
    private ValueLazy<ObjCDictionaryLiteral, List<(Expr Key, Expr Value)>> _keyValueElements;

    internal unsafe ObjCDictionaryLiteral(CXCursor handle) : base(handle, CXCursor_UnexposedExpr, CX_StmtClass_ObjCDictionaryLiteral)
    {
        Debug.Assert((NumChildren % 2) == 0);
        _dictWithObjectsMethod = new ValueLazy<ObjCDictionaryLiteral, ObjCMethodDecl>(&DictWithObjectsMethodFactory);

        _keyValueElements = new ValueLazy<ObjCDictionaryLiteral, List<(Expr Key, Expr Value)>>(&KeyValueElementsFactory);
    }

    public ObjCMethodDecl DictWithObjectsMethod => _dictWithObjectsMethod.GetValue(this);

    public IReadOnlyList<(Expr Key, Expr Value)> KeyValueElements => _keyValueElements.GetValue(this);

    public uint NumElements => NumChildren / 2;

    private static unsafe List<(Expr Key, Expr Value)> KeyValueElementsFactory(ObjCDictionaryLiteral self) {
            var numChildren = self.Handle.NumChildren;
            var keyValueElements = new List<(Expr Key, Expr Value)>(numChildren / 2);

            for (var i = 0; i < numChildren; i += 2)
            {
                keyValueElements.Add(((Expr)self.Children[i + 0], (Expr)self.Children[i + 1]));
            }

            return keyValueElements;
        }

    private static unsafe ObjCMethodDecl DictWithObjectsMethodFactory(ObjCDictionaryLiteral self) => self.TranslationUnit.GetOrCreate<ObjCMethodDecl>(self.Handle.Referenced);
}
