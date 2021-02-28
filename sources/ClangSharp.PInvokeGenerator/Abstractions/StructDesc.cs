using System;
using System.Runtime.InteropServices;

namespace ClangSharp.Abstractions
{
    public struct StructDesc<TCustomAttrGeneratorData>
    {
        public string AccessSpecifier;
        public string EscapedName;
        public string NativeType;
        public string NativeInheritance;
        public StructLayoutAttribute Layout;
        public Guid? Uuid;
        public bool IsUnsafe;
        public bool HasVtbl;
        public Action<TCustomAttrGeneratorData> WriteCustomAttrs;
        public TCustomAttrGeneratorData CustomAttrGeneratorData;
    }
}
