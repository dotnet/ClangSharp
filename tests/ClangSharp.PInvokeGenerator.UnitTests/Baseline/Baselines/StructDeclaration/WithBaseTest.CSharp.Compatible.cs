using System;
using System.Runtime.InteropServices;

namespace ClangSharp.Test
{
    public unsafe partial struct IThing : IThing.Interface
    {
        public void** lpVtbl;

        [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
        public delegate int _DoWork(IThing* pThis);

        public int DoWork()
        {
            fixed (IThing* pThis = &this)
            {
                return Marshal.GetDelegateForFunctionPointer<_DoWork>((IntPtr)(lpVtbl[0]))(pThis);
            }
        }

        public interface Interface : INativeGuid
        {
            int DoWork();
        }
    }

    public partial struct PlainThing : IDisposable
    {
        public int value;
    }
}
