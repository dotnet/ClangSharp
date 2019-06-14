namespace ClangSharp.Interop
{
    public enum CXDiagnosticDisplayOptions
    {
        CXDiagnostic_DisplaySourceLocation = 0x01,
        CXDiagnostic_DisplayColumn = 0x02,
        CXDiagnostic_DisplaySourceRanges = 0x04,
        CXDiagnostic_DisplayOption = 0x08,
        CXDiagnostic_DisplayCategoryId = 0x10,
        CXDiagnostic_DisplayCategoryName = 0x20,
    }
}
