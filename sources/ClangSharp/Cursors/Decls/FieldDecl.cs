using ClangSharp.Interop;

namespace ClangSharp
{
    public sealed class FieldDecl : DeclaratorDecl, IMergeable<FieldDecl>
    {
        internal FieldDecl(CXCursor handle) : base(handle, CXCursorKind.CXCursor_FieldDecl)
        {
        }

        public int BitWidthValue => Handle.FieldDeclBitWidth;

        public bool IsBitField => Handle.IsBitField;

        public bool IsMutable => Handle.CXXField_IsMutable;

        public RecordDecl Parent => (RecordDecl)CursorParent;
    }
}
