using System;
using System.Diagnostics;

namespace ClangSharp
{
    internal class Ref : Cursor
    {
        public static new Ref Create(CXCursor handle, Cursor parent)
        {
            switch (handle.Kind)
            {
                case CXCursorKind.CXCursor_TypeRef:
                {
                    return new TypeRef(handle, parent);
                }

                case CXCursorKind.CXCursor_CXXBaseSpecifier:
                {
                    return new CXXBaseSpecifier(handle, parent);
                }

                case CXCursorKind.CXCursor_TemplateRef:
                {
                    return new TemplateRef(handle, parent);
                }

                case CXCursorKind.CXCursor_NamespaceRef:
                {
                    return new NamespaceRef(handle, parent);
                }

                case CXCursorKind.CXCursor_MemberRef:
                {
                    return new MemberRef(handle, parent);
                }

                case CXCursorKind.CXCursor_OverloadedDeclRef:
                {
                    return new OverloadedDeclRef(handle, parent);
                }

                default:
                {
                    Debug.WriteLine($"Unhandled reference kind: {handle.KindSpelling}.");
                    Debugger.Break();
                    return new Ref(handle, parent);
                }
            }
        }

        private readonly Lazy<Cursor> _definition;
        private readonly Lazy<Type> _type;

        protected Ref(CXCursor handle, Cursor parent) : base(handle, parent)
        {
            Debug.Assert(handle.IsReference);

            _definition = new Lazy<Cursor>(() => {
                var cursor = TranslationUnit.GetOrCreateCursor(handle.Definition, () => Create(handle.Definition, this));
                cursor.Visit(clientData: default);
                return cursor;
            });
            _type = new Lazy<Type>(() => TranslationUnit.GetOrCreateType(Handle.Type, () => Type.Create(Handle.Type, TranslationUnit)));
        }

        public Cursor Definition => _definition.Value;

        public Type Type => _type.Value;
    }
}
