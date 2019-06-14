using System.Diagnostics;
using ClangSharp.Interop;

namespace ClangSharp
{
    internal sealed class ConstantArrayType : ArrayType
    {
        public ConstantArrayType(CXType handle, TranslationUnit translationUnit) : base(handle, translationUnit)
        {
            Debug.Assert(handle.kind == CXTypeKind.CXType_ConstantArray);
        }

        public long Size => Handle.ArraySize;
    }
}
