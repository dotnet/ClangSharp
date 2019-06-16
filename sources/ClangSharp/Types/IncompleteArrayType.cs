using ClangSharp.Interop;

namespace ClangSharp
{
    public sealed class IncompleteArrayType : ArrayType
    {
        internal IncompleteArrayType(CXType handle) : base(handle, CXTypeKind.CXType_IncompleteArray)
        {
        }
    }
}
