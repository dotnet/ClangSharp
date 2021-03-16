namespace ClangSharp.Abstractions
{
    internal struct ConstantDesc
    {
        public AccessSpecifier AccessSpecifier { get; set; }
        public string TypeName { get; set; }
        public string EscapedName { get; set; }
        public string NativeTypeName { get; set; }
        public ConstantKind Kind { get; set; }
    }
}
