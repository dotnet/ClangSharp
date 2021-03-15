namespace ClangSharp.Abstractions
{
    internal struct FieldDesc
    {
        public string AccessSpecifier { get; set; }
        public string NativeTypeName { get; set; }
        public string EscapedName { get; set; }
        public int? Offset { get; set; }
        public bool NeedsNewKeyword { get; set; }
        public string InheritedFrom { get; set; }
    }
}
