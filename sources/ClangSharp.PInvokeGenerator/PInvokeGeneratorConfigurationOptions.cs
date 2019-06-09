using System;

namespace ClangSharp
{
    [Flags]
    public enum PInvokeGeneratorConfigurationOptions
    {
        None = 0x00000000,

        GenerateMultipleFiles = 0x00000001,
    }
}
