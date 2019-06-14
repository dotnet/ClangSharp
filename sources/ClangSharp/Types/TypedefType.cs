using System;
using System.Diagnostics;
using ClangSharp.Interop;

namespace ClangSharp
{
    public sealed class TypedefType : Type
    {
        private readonly Lazy<TypedefNameDecl> _decl;

        public TypedefType(CXType handle, TranslationUnit translationUnit) : base(handle, translationUnit)
        {
            Debug.Assert(handle.kind == CXTypeKind.CXType_Typedef);

            _decl = new Lazy<TypedefNameDecl>(() => {
                var cursor = translationUnit.GetOrCreateCursor(Handle.Declaration, () => Cursor.Create(Handle.Declaration, translationUnit));
                cursor?.Visit(clientData: default);
                return (TypedefNameDecl)cursor;
            });
        }

        public TypedefNameDecl Decl => _decl.Value;

        public string Name => Handle.TypedefName.ToString();

        public bool IsTransparentTag => Handle.IsTransparentTagTypedef;

        public Type UnderlyingType => Decl.UnderlyingType;
    }
}
