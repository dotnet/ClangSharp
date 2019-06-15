using System.Diagnostics;
using ClangSharp.Interop;

namespace ClangSharp
{
    public sealed class EnumType : TagType
    {
        public EnumType(CXType handle, TranslationUnitDecl translationUnit) : base(handle, translationUnit)
        {
            Debug.Assert(handle.kind == CXTypeKind.CXType_Enum);
        }
    }
}
