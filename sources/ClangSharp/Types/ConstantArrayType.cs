using System.Diagnostics;
using ClangSharp.Interop;

namespace ClangSharp
{
    public sealed class ConstantArrayType : ArrayType
    {
        public ConstantArrayType(CXType handle, TranslationUnitDecl translationUnit) : base(handle, translationUnit)
        {
            Debug.Assert(handle.kind == CXTypeKind.CXType_ConstantArray);
        }

        public long Size => Handle.ArraySize;
    }
}
