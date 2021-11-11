// Copyright (c) .NET Foundation and Contributors. All Rights Reserved. Licensed under the MIT License (MIT). See License.md in the repository root for more information.

using System;
using System.Diagnostics;
using ClangSharp.Interop;

namespace ClangSharp
{
    public class Ref : Cursor
    {
        private readonly Lazy<NamedDecl> _referenced;
        private readonly Lazy<Type> _type;

        private protected Ref(CXCursor handle, CXCursorKind expectedCursorKind) : base(handle, expectedCursorKind)
        {
            _referenced = new Lazy<NamedDecl>(() => TranslationUnit.GetOrCreate<NamedDecl>(Handle.Referenced));
            _type = new Lazy<Type>(() => TranslationUnit.GetOrCreate<Type>(Handle.Type));
        }

        public NamedDecl Referenced => _referenced.Value;

        public Type Type => _type.Value;

        internal static new Ref Create(CXCursor handle)
        {
            Ref result;

            switch (handle.Kind)
            {
                case CXCursorKind.CXCursor_CXXBaseSpecifier:
                {
                    result = new CXXBaseSpecifier(handle);
                    break;
                }

                case CXCursorKind.CXCursor_ObjCSuperClassRef:
                case CXCursorKind.CXCursor_ObjCProtocolRef:
                case CXCursorKind.CXCursor_ObjCClassRef:
                case CXCursorKind.CXCursor_TypeRef:
                case CXCursorKind.CXCursor_TemplateRef:
                case CXCursorKind.CXCursor_NamespaceRef:
                case CXCursorKind.CXCursor_MemberRef:
                case CXCursorKind.CXCursor_LabelRef:
                case CXCursorKind.CXCursor_OverloadedDeclRef:
                case CXCursorKind.CXCursor_VariableRef:
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
}
