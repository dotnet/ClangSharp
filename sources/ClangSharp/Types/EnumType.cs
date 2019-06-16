using ClangSharp.Interop;

namespace ClangSharp
{
    public sealed class EnumType : TagType
    {
        internal EnumType(CXType handle) : base(handle, CXTypeKind.CXType_Enum)
        {
        }
    }
}
