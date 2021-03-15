using System;
using System.Runtime.InteropServices;

namespace ClangSharp.Abstractions
{
    internal struct StructDesc<TCustomAttrGeneratorData>
    {
        public string AccessSpecifier { get; set; }
        public string EscapedName { get; set; }
        public string NativeType { get; set; }
        public string NativeInheritance { get; set; }
        public StructLayoutAttribute Layout { get; set; }
        public Guid? Uuid { get; set; }
        public StructFlags Flags { get; set; }

        public bool IsUnsafe
        {
            get => (Flags & StructFlags.IsUnsafe) != 0;
            set => Flags = value ? Flags | StructFlags.IsUnsafe : Flags ^ StructFlags.IsUnsafe;
        }

        public bool HasVtbl
        {
            get => (Flags & StructFlags.HasVtbl) != 0;
            set => Flags = value ? Flags | StructFlags.HasVtbl : Flags ^ StructFlags.HasVtbl;
        }

        public Action<TCustomAttrGeneratorData> WriteCustomAttrs { get; set; }
        public TCustomAttrGeneratorData CustomAttrGeneratorData { get; set; }
    }
}
