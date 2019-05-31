using System;
using System.Runtime.InteropServices;

namespace ClangSharp
{
    public partial struct CXIdxEntityInfo
    {
        public CXIdxEntityKind kind;
        public CXIdxEntityCXXTemplateKind templateKind;
        public CXIdxEntityLanguage lang;
        [MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(StringMarshaler))] public string name;
        [MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(StringMarshaler))] public string USR;
        public CXCursor cursor;
        public IntPtr attributes;
        public uint numAttributes;
    }
}
