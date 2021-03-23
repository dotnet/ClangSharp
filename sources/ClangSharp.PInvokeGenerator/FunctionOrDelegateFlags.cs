using System;

namespace ClangSharp
{
    [Flags]
    public enum FunctionOrDelegateFlags
    {
        IsVirtual = 1 << 0,
        IsDllImport = 1 << 1,
        HasFnPtrCodeGen = 1 << 2,
        IsAggressivelyInlined = 1 << 3,
        SetLastError = 1 << 4,
        IsCxx = 1 << 5,
        NeedsNewKeyword = 1 << 6,
        IsUnsafe = 1 << 7,
        IsCtxCxxRecord = 1 << 8,
        IsCxxRecordCtxUnsafe = 1 << 9,
        IsMemberFunction = 1 << 10,
        IsStatic = 1 << 11,
        IsNotStatic = 1 << 12
    }
}
