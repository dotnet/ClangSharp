using ClangSharp.Interop;

namespace ClangSharp.Abstractions
{
    public struct EnumDesc
    {
        public AccessSpecifier AccessSpecifier { get; set; }
        public string TypeName { get; set; }
        public string EscapedName { get; set; }
        public string NativeType { get; set; }
        public CXSourceLocation? Location { get; set; }
    }
}