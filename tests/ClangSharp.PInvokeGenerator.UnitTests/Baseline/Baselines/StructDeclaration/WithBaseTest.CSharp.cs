using System.Runtime.CompilerServices;

namespace ClangSharp.Test
{
    public unsafe partial struct IThing : IThing.Interface
    {
        public void** lpVtbl;

        public int DoWork()
        {
            return ((delegate* unmanaged[Thiscall]<IThing*, int>)(lpVtbl[0]))((IThing*)Unsafe.AsPointer(ref this));
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
