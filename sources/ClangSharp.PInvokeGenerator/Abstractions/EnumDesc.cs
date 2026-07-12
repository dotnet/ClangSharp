using System;
using ClangSharp.Interop;

namespace ClangSharp.Abstractions;

internal struct EnumDesc
{
    public AccessSpecifier AccessSpecifier { get; set; }
    public string TypeName { get; set; }
    public string EscapedName { get; set; }
    public string NativeType { get; set; }
    public CXSourceLocation? Location { get; set; }
    public EnumFlags Flags { get; set; }

    public bool IsNested
    {
        readonly get
        {
            return GetFlag(EnumFlags.Nested);
        }

        set
        {
            SetFlag(EnumFlags.Nested, value);
        }
    }

    private readonly bool GetFlag(EnumFlags flag) => (Flags & flag) != 0;

    private void SetFlag(EnumFlags flag, bool value) => Flags = value ? Flags | flag : Flags & ~flag;

    public Action<object> WriteCustomAttrs { get; set; }
    public object CustomAttrGeneratorData { get; set; }
}
