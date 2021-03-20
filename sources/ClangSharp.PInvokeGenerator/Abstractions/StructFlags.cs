using System;

namespace ClangSharp.Abstractions
{
    [Flags]
    public enum StructFlags
    {
        IsUnsafe = 1 << 0,
        HasVtbl = 1 << 1
    }
}
