using System.Diagnostics;
using ClangSharp.Interop;

namespace ClangSharp
{
    internal sealed class FieldDecl : DeclaratorDecl
    {
        public FieldDecl(CXCursor handle, Cursor parent) : base(handle, parent)
        {
            Debug.Assert(handle.Kind == CXCursorKind.CXCursor_FieldDecl);
        }

        public int BitWidthValue => Handle.FieldDeclBitWidth;

        public bool IsBitField => Handle.IsBitField;

        public bool IsMutable => Handle.CXXField_IsMutable;

        public long Offset => Handle.OffsetOfField;
    }
}
