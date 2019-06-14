using System.Collections.Generic;
using System.Diagnostics;
using ClangSharp.Interop;

namespace ClangSharp
{
    internal sealed class EnumDecl : TagDecl
    {
        private readonly List<EnumConstantDecl> _enumerators = new List<EnumConstantDecl>();

        public EnumDecl(CXCursor handle, Cursor parent) : base(handle, parent)
        {
            Debug.Assert(handle.Kind == CXCursorKind.CXCursor_EnumDecl);
            IntegerType = TranslationUnit.GetOrCreateType(Handle.EnumDecl_IntegerType, () => Type.Create(Handle.EnumDecl_IntegerType, TranslationUnit));
        }

        public IReadOnlyList<EnumConstantDecl> Enumerators => _enumerators;

        public Type IntegerType { get; }

        public bool IsScoped => Handle.EnumDecl_IsScoped;

        protected override Decl GetOrAddDecl(CXCursor childHandle)
        {
            var decl = base.GetOrAddDecl(childHandle);

            if (decl is EnumConstantDecl enumConstantDecl)
            {
                _enumerators.Add(enumConstantDecl);
            }

            return decl;
        }
    }
}
