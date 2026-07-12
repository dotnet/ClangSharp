// Copyright (c) .NET Foundation and Contributors. All Rights Reserved. Licensed under the MIT License (MIT). See License.md in the repository root for more information.

using System.Diagnostics;
using ClangSharp.Interop;
using static ClangSharp.Interop.CXCursorKind;

namespace ClangSharp;

public class Ref : Cursor
{
    private ValueLazy<Ref, NamedDecl> _referenced;
    private ValueLazy<Ref, Type> _type;

    private protected unsafe Ref(CXCursor handle, CXCursorKind expectedCursorKind) : base(handle, expectedCursorKind)
    {
        _referenced = new ValueLazy<Ref, NamedDecl>(&ReferencedFactory);
        _type = new ValueLazy<Ref, Type>(&TypeFactory);
    }

    public NamedDecl Referenced => _referenced.GetValue(this);

    public Type Type => _type.GetValue(this);

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

    private static unsafe Type TypeFactory(Ref self) => self.TranslationUnit.GetOrCreate<Type>(self.Handle.Type);

    private static unsafe NamedDecl ReferencedFactory(Ref self) => self.TranslationUnit.GetOrCreate<NamedDecl>(self.Handle.Referenced);
}
