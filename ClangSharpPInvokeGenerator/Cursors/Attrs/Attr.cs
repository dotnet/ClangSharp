using System.Diagnostics;
using ClangSharp;

namespace ClangSharpPInvokeGenerator
{
    internal class Attr : Cursor
    {
        protected Attr(CXCursor handle, Cursor parent) : base(handle, parent)
        {
            Debug.Assert(handle.IsAttribute);
        }
    }
}
