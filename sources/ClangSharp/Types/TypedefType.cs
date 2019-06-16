using System;
using ClangSharp.Interop;

namespace ClangSharp
{
    public sealed class TypedefType : Type
    {
        private readonly Lazy<TypedefNameDecl> _decl;

        internal TypedefType(CXType handle) : base(handle, CXTypeKind.CXType_Typedef)
        {
            _decl = new Lazy<TypedefNameDecl>(() => TranslationUnit.GetOrCreate<TypedefNameDecl>(Handle.Declaration));
        }

        public TypedefNameDecl Decl => _decl.Value;
    }
}
