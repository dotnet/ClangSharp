namespace ClangSharp
{
    using System;
    using System.Runtime.InteropServices;

    public enum CXSymbolRole
    {
        CXSymbolRole_None = 0,
        CXSymbolRole_Declaration = 1,
        CXSymbolRole_Definition = 2,
        CXSymbolRole_Reference = 4,
        CXSymbolRole_Read = 8,
        CXSymbolRole_Write = 16,
        CXSymbolRole_Call = 32,
        CXSymbolRole_Dynamic = 64,
        CXSymbolRole_AddressOf = 128,
        CXSymbolRole_Implicit = 256,
    }
}
