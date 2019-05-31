using System.Runtime.InteropServices;

namespace ClangSharp
{
    public partial struct CXUnsavedFile
    {
        [MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(StringMarshaler))] public string Filename;
        [MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(StringMarshaler))] public string Contents;
        public uint Length;
    }
}
