using ClangSharp.Interop;

namespace ClangSharp
{
    public sealed class DependentSizedArrayType : ArrayType
    {
        internal DependentSizedArrayType(CXType handle) : base(handle, CXTypeKind.CXType_DependentSizedArray)
        {
        }
    }
}
