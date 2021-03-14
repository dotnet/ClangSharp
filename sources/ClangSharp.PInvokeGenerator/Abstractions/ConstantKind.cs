using System;

namespace ClangSharp.Abstractions
{
    [Flags]
    internal enum ConstantKind
    {
        None = 0,
        ReadOnly = 1 << 0,
        Enumerator = 1 << 1,
        PrimitiveConstant = 1 << 2,
        NonPrimitiveConstant = 1 << 3
    }
}
