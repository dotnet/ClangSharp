using ClangSharp.Interop;

namespace ClangSharp
{
    public sealed class LValueReferenceType : ReferenceType
    {
        internal LValueReferenceType(CXType handle) : base(handle, CXTypeKind.CXType_LValueReference)
        {
        }
    }
}
