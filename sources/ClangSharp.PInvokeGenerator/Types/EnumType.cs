using System.Diagnostics;
using ClangSharp.Interop;

namespace ClangSharp
{
    internal sealed class EnumType : TagType
    {
        public EnumType(CXType handle, TranslationUnit translationUnit) : base(handle, translationUnit)
        {
            Debug.Assert(handle.kind == CXTypeKind.CXType_Enum);
        }
    }
}
