using ClangSharp.Interop;

namespace ClangSharp
{
    public sealed class CXXConversionDecl : CXXMethodDecl
    {
        internal CXXConversionDecl(CXCursor handle) : base(handle, CXCursorKind.CXCursor_ConversionFunction)
        {
        }
    }
}
