using System.Diagnostics;

namespace ClangSharp
{
    internal sealed class ParmVarDecl : VarDecl
    {
        private int _index = -1;

        public ParmVarDecl(CXCursor handle, Cursor parent) : base(handle, parent)
        {
            Debug.Assert(handle.Kind == CXCursorKind.CXCursor_ParmDecl);
        }

        public int Index
        {
            get
            {
                Debug.Assert(_index >= 0);
                return _index;
            }

            set
            {
                Debug.Assert((_index == -1) && (value >= 0));
                _index = value;
            }
        }
    }
}
