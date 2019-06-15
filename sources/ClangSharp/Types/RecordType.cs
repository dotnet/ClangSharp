using System.Diagnostics;
using ClangSharp.Interop;

namespace ClangSharp
{
    public sealed class RecordType : TagType
    {
        public RecordType(CXType handle, TranslationUnitDecl translationUnit) : base(handle, translationUnit)
        {
            Debug.Assert(handle.kind == CXTypeKind.CXType_Record);
        }
    }
}
