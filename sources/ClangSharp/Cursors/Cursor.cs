// Copyright (c) .NET Foundation and Contributors. All Rights Reserved. Licensed under the MIT License (MIT). See License.md in the repository root for more information.

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using ClangSharp.Interop;
using static ClangSharp.Interop.CXChildVisitResult;

namespace ClangSharp;

[DebuggerDisplay("{Handle.DebuggerDisplayString,nq}")]
public unsafe class Cursor : IEquatable<Cursor>
{
    private ValueLazy<Cursor, string> _kindSpelling;
    private ValueLazy<Cursor, Cursor?> _lexicalParentCursor;
    private ValueLazy<Cursor, Cursor?> _semanticParentCursor;
    private ValueLazy<Cursor, string> _spelling;
    private ValueLazy<Cursor, TranslationUnit> _translationUnit;
    private List<Cursor>? _cursorChildren;

    private protected Cursor(CXCursor handle, CXCursorKind expectedCursorKind)
    {
        if (handle.kind != expectedCursorKind)
        {
            throw new ArgumentOutOfRangeException(nameof(handle));
        }
        Handle = handle;

        _kindSpelling = new ValueLazy<Cursor, string>(&KindSpellingFactory);
        _lexicalParentCursor = new ValueLazy<Cursor, Cursor?>(&LexicalParentCursorFactory);
        _semanticParentCursor = new ValueLazy<Cursor, Cursor?>(&SemanticParentCursorFactory);
        _spelling = new ValueLazy<Cursor, string>(&SpellingFactory);
        _translationUnit = new ValueLazy<Cursor, TranslationUnit>(&TranslationUnitFactory);
    }

    public IReadOnlyList<Cursor> CursorChildren
    {
        get
        {
            if (_cursorChildren is null)
            {
                var cursorChildrenHandle = GCHandle.Alloc(new List<Cursor>());

                var client_data = stackalloc nint[2] {
                    GCHandle.ToIntPtr(cursorChildrenHandle),
                    TranslationUnit.Handle.Handle
                };

                _ = clang.visitChildren(Handle, &Visitor, client_data);

                var cursorChildren = (List<Cursor>?)cursorChildrenHandle.Target;
                Debug.Assert(cursorChildren is not null);

                _cursorChildren = cursorChildren;
                cursorChildrenHandle.Free();

                [UnmanagedCallersOnly(CallConvs = [typeof(CallConvCdecl)])]
                static CXChildVisitResult Visitor(CXCursor cursor, CXCursor parent, void* client_data)
                {
                    var cursorChildren = (List<Cursor>?)GCHandle.FromIntPtr(((nint*)client_data)[0]).Target;
                    Debug.Assert(cursorChildren is not null);

                    var translationUnit = TranslationUnit.GetOrCreate((CXTranslationUnitImpl*)((nint*)client_data)[1]);
                    var cursorChild = translationUnit.GetOrCreate<Cursor>(cursor);

                    cursorChildren!.Add(cursorChild);
                    return CXChildVisit_Continue;
                }
            }

            return _cursorChildren!;
        }
    }

    public CXCursorKind CursorKind => Handle.kind;

    public string CursorKindSpelling => _kindSpelling.GetValue(this);

    public CXSourceRange Extent => Handle.Extent;

    public CXCursor Handle { get; }

    public Cursor? LexicalParentCursor => _lexicalParentCursor.GetValue(this);

    public CXSourceLocation Location => Handle.Location;

    public Cursor? SemanticParentCursor => _semanticParentCursor.GetValue(this);

    public string Spelling => _spelling.GetValue(this);

    public TranslationUnit TranslationUnit => _translationUnit.GetValue(this);

    public static bool operator ==(Cursor? left, Cursor? right) => (left is not null) ? ((right is not null) && (left.Handle == right.Handle)) : (right is null);

    public static bool operator !=(Cursor? left, Cursor? right) => (left is not null) ? ((right is null) || (left.Handle != right.Handle)) : (right is not null);

    internal static Cursor Create(CXCursor handle)
    {
        Cursor result;

        if (handle.IsDeclaration || handle.IsTranslationUnit)
        {
            result = Decl.Create(handle);
        }
        else if (handle.IsReference)
        {
            result = Ref.Create(handle);
        }
        else if (handle.IsExpression || handle.IsStatement)
        {
            result = Stmt.Create(handle);
        }
        else if (handle.IsAttribute)
        {
            result = Attr.Create(handle);
        }
        else if (handle.IsPreprocessing)
        {
            result = PreprocessedEntity.Create(handle);
        }
        else
        {
            Debug.WriteLine($"Unhandled cursor kind: {handle.KindSpelling}.");
            result = new Cursor(handle, handle.Kind);
        }

        return result;
    }

    public override bool Equals(object? obj) => (obj is Cursor other) && Equals(other);

    public bool Equals(Cursor? other) => this == other;

    public override int GetHashCode() => Handle.GetHashCode();

    public override string ToString() => Spelling;

    private static unsafe TranslationUnit TranslationUnitFactory(Cursor self) => TranslationUnit.GetOrCreate(self.Handle.TranslationUnit);

    private static unsafe string SpellingFactory(Cursor self) => self.Handle.Spelling.ToString();

    private static unsafe Cursor? SemanticParentCursorFactory(Cursor self) => !self.Handle.SemanticParent.IsNull ? self.TranslationUnit.GetOrCreate<Cursor>(self.Handle.SemanticParent) : null;

    private static unsafe Cursor? LexicalParentCursorFactory(Cursor self) => !self.Handle.LexicalParent.IsNull ? self.TranslationUnit.GetOrCreate<Cursor>(self.Handle.LexicalParent) : null;

    private static unsafe string KindSpellingFactory(Cursor self) => self.Handle.KindSpelling.ToString();
}
