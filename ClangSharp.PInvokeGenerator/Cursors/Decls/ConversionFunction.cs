using System.Diagnostics;

namespace ClangSharp
{
    internal sealed class ConversionFunction : Decl
    {
        public ConversionFunction(CXCursor handle, Cursor parent) : base(handle, parent)
        {
            Debug.Assert(handle.Kind == CXCursorKind.CXCursor_ConversionFunction);
        }
    }
}
