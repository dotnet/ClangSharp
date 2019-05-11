namespace ClangSharp
{
    public enum CXCallingConv
    {
        CXCallingConv_Default = 0,
        CXCallingConv_C = 1,
        CXCallingConv_X86StdCall = 2,
        CXCallingConv_X86FastCall = 3,
        CXCallingConv_X86ThisCall = 4,
        CXCallingConv_X86Pascal = 5,
        CXCallingConv_AAPCS = 6,
        CXCallingConv_AAPCS_VFP = 7,
        CXCallingConv_X86RegCall = 8,
        CXCallingConv_IntelOclBicc = 9,
        CXCallingConv_Win64 = 10,
        CXCallingConv_X86_64Win64 = CXCallingConv_Win64,
        CXCallingConv_X86_64SysV = 11,
        CXCallingConv_X86VectorCall = 12,
        CXCallingConv_Swift = 13,
        CXCallingConv_PreserveMost = 14,
        CXCallingConv_PreserveAll = 15,
        CXCallingConv_AArch64VectorCall = 16,
        CXCallingConv_Invalid = 100,
        CXCallingConv_Unexposed = 200,
    }
}
