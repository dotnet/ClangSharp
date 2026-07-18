using System.Runtime.InteropServices;

namespace ClangSharp.Test
{
    public partial struct tagMETARECORD
    {
        public int rdSize;
    }

    public static unsafe partial class Methods
    {
        [DllImport("ClangSharpPInvokeGenerator", CallingConvention = CallingConvention.Cdecl, EntryPoint = "?PlayMetaFileRecord@@YAXPEFAUtagMETARECORD@@@Z", ExactSpelling = true)]
        public static extern void PlayMetaFileRecord([NativeTypeName("LPMETARECORD")] tagMETARECORD* lpMR);
    }
}
