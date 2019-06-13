using System.Diagnostics;
using ClangSharp.Interop;

namespace ClangSharp
{
    internal sealed class RecordType : TagType
    {
        public RecordType(CXType handle, TranslationUnit translationUnit) : base(handle, translationUnit)
        {
            Debug.Assert(handle.kind == CXTypeKind.CXType_Record);
        }
    }
}
