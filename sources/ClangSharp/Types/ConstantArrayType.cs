using ClangSharp.Interop;

namespace ClangSharp
{
    public sealed class ConstantArrayType : ArrayType
    {
        internal ConstantArrayType(CXType handle) : base(handle, CXTypeKind.CXType_ConstantArray)
        {
        }

        public long Size => Handle.ArraySize;
    }
}
