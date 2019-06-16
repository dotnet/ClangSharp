using ClangSharp.Interop;

namespace ClangSharp
{
    public sealed class BuiltinType : Type
    {
        internal BuiltinType(CXType handle, CXTypeKind expectedKind) : base(handle, expectedKind)
        {
        }
    }
}
