namespace ClangSharp.Interop
{
    public enum CXSymbolRole
    {
        CXSymbolRole_None = 0,
        CXSymbolRole_Declaration = 1 << 0,
        CXSymbolRole_Definition = 1 << 1,
        CXSymbolRole_Reference = 1 << 2,
        CXSymbolRole_Read = 1 << 3,
        CXSymbolRole_Write = 1 << 4,
        CXSymbolRole_Call = 1 << 5,
        CXSymbolRole_Dynamic = 1 << 6,
        CXSymbolRole_AddressOf = 1 << 7,
        CXSymbolRole_Implicit = 1 << 8,
    }
}
