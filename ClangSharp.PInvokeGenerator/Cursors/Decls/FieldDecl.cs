using System.Diagnostics;

namespace ClangSharp
{
    internal sealed class FieldDecl : Decl
    {
        public FieldDecl(CXCursor handle, Cursor parent) : base(handle, parent)
        {
            Debug.Assert(handle.Kind == CXCursorKind.CXCursor_FieldDecl);
        }

        public int BitWidth => Handle.FieldDeclBitWidth;

        public bool IsBitField => Handle.IsBitField;

        public bool IsMutable => Handle.CXXField_IsMutable;
    }
}
