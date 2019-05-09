namespace ClangSharp
{
    using System;
    using System.Runtime.InteropServices;

    public partial struct CXUnsavedFile
    {
        [MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(StringMarshaler))] public string Filename;
        [MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(StringMarshaler))] public string Contents;
        public uint Length;
    }
}
