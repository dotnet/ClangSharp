using System;
using ClangSharp.Interop;

namespace ClangSharp.Abstractions;

public struct EnumDesc
{
    public AccessSpecifier AccessSpecifier { get; set; }
    public string TypeName { get; set; }
    public string EscapedName { get; set; }
    public string NativeType { get; set; }
    public CXSourceLocation? Location { get; set; }
    public EnumFlags Flags { get; set; }

    public bool IsNested
    {
        get
        {
            return (Flags & EnumFlags.Nested) != 0;
        }

        set
        {
            Flags = value ? Flags | EnumFlags.Nested : Flags & ~EnumFlags.Nested;
        }
    }
    public Action<object> WriteCustomAttrs { get; set; }
    public object CustomAttrGeneratorData { get; set; }
}
