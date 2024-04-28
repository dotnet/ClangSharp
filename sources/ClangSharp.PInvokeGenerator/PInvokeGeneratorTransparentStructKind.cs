namespace ClangSharp;

public enum PInvokeGeneratorTransparentStructKind
{
    Unknown = 0,
    Typedef = 1,
    Handle = 2,
    Boolean = 3,
    HandleWin32 = 4,
    TypedefHex = 5,
    HandleVulkan = 6,
    FnPtr = 7,
}
