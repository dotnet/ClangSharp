using ClangSharp.Interop;

namespace ClangSharp
{
    public sealed class RecordType : TagType
    {
        internal RecordType(CXType handle) : base(handle, CXTypeKind.CXType_Record)
        {
        }
    }
}
