// Copyright (c) .NET Foundation and Contributors. All Rights Reserved. Licensed under the MIT License (MIT). See License.md in the repository root for more information.

using System.Diagnostics;
using ClangSharp.Interop;
using static ClangSharp.Interop.CXCursorKind;

namespace ClangSharp;

public class Ref : Cursor
{
    private readonly ValueLazy<NamedDecl> _referenced;
    private readonly ValueLazy<Type> _type;

    private protected Ref(CXCursor handle, CXCursorKind expectedCursorKind) : base(handle, expectedCursorKind)
    {
        _referenced = new ValueLazy<NamedDecl>(() => TranslationUnit.GetOrCreate<NamedDecl>(Handle.Referenced));
        _type = new ValueLazy<Type>(() => TranslationUnit.GetOrCreate<Type>(Handle.Type));
    }

    public NamedDecl Referenced => _referenced.Value;

    public Type Type => _type.Value;

    internal static new Ref Create(CXCursor handle)
    {
        Ref result;

        switch (handle.Kind)
        {
            case CXCursor_CXXBaseSpecifier:
            {
                result = new CXXBaseSpecifier(handle);
                break;
            }

            case CXCursor_OverloadedDeclRef:
            {
                result = new OverloadedDeclRef(handle);
                break;
            }

            case CXCursor_ObjCSuperClassRef:
            case CXCursor_ObjCProtocolRef:
            case CXCursor_ObjCClassRef:
            case CXCursor_TypeRef:
            case CXCursor_TemplateRef:
            case CXCursor_NamespaceRef:
            case CXCursor_MemberRef:
            case CXCursor_LabelRef:
            case CXCursor_VariableRef:
            {
                result = new Ref(handle, handle.Kind);
                break;
            }

            default:
            {
                Debug.WriteLine($"Unhandled reference kind: {handle.KindSpelling}.");
                result = new Ref(handle, handle.Kind);
                break;
            }
        }

        return result;
    }
}
