using ClangSharp.Interop;

namespace ClangSharp
{
    public class TypeWithKeyword : Type
    {
        private protected TypeWithKeyword(CXType handle, CXTypeKind expectedKind) : base(handle, expectedKind)
        {
        }
    }
}
