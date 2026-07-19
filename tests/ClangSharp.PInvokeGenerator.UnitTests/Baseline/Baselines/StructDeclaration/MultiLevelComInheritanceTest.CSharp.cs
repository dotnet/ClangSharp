using System.Runtime.CompilerServices;

namespace ClangSharp.Test
{
    public unsafe partial struct IUnknown
    {
        public void** lpVtbl;

        [VtblIndex(0)]
        public int QueryInterface()
        {
            return ((delegate* unmanaged[Thiscall]<IUnknown*, int>)(lpVtbl[0]))((IUnknown*)Unsafe.AsPointer(ref this));
        }

        [VtblIndex(1)]
        [return: NativeTypeName("unsigned int")]
        public uint AddRef()
        {
            return ((delegate* unmanaged[Thiscall]<IUnknown*, uint>)(lpVtbl[1]))((IUnknown*)Unsafe.AsPointer(ref this));
        }

        [VtblIndex(2)]
        [return: NativeTypeName("unsigned int")]
        public uint Release()
        {
            return ((delegate* unmanaged[Thiscall]<IUnknown*, uint>)(lpVtbl[2]))((IUnknown*)Unsafe.AsPointer(ref this));
        }
    }

    [NativeTypeName("struct ISequentialStream : IUnknown")]
    public unsafe partial struct ISequentialStream
    {
        public void** lpVtbl;

        [VtblIndex(0)]
        public int QueryInterface()
        {
            return ((delegate* unmanaged[Thiscall]<ISequentialStream*, int>)(lpVtbl[0]))((ISequentialStream*)Unsafe.AsPointer(ref this));
        }

        [VtblIndex(1)]
        [return: NativeTypeName("unsigned int")]
        public uint AddRef()
        {
            return ((delegate* unmanaged[Thiscall]<ISequentialStream*, uint>)(lpVtbl[1]))((ISequentialStream*)Unsafe.AsPointer(ref this));
        }

        [VtblIndex(2)]
        [return: NativeTypeName("unsigned int")]
        public uint Release()
        {
            return ((delegate* unmanaged[Thiscall]<ISequentialStream*, uint>)(lpVtbl[2]))((ISequentialStream*)Unsafe.AsPointer(ref this));
        }

        [VtblIndex(3)]
        public int Read()
        {
            return ((delegate* unmanaged[Thiscall]<ISequentialStream*, int>)(lpVtbl[3]))((ISequentialStream*)Unsafe.AsPointer(ref this));
        }

        [VtblIndex(4)]
        public int Write()
        {
            return ((delegate* unmanaged[Thiscall]<ISequentialStream*, int>)(lpVtbl[4]))((ISequentialStream*)Unsafe.AsPointer(ref this));
        }
    }

    [NativeTypeName("struct IStream : ISequentialStream")]
    public unsafe partial struct IStream
    {
        public void** lpVtbl;

        [VtblIndex(0)]
        public int QueryInterface()
        {
            return ((delegate* unmanaged[Thiscall]<IStream*, int>)(lpVtbl[0]))((IStream*)Unsafe.AsPointer(ref this));
        }

        [VtblIndex(1)]
        [return: NativeTypeName("unsigned int")]
        public uint AddRef()
        {
            return ((delegate* unmanaged[Thiscall]<IStream*, uint>)(lpVtbl[1]))((IStream*)Unsafe.AsPointer(ref this));
        }

        [VtblIndex(2)]
        [return: NativeTypeName("unsigned int")]
        public uint Release()
        {
            return ((delegate* unmanaged[Thiscall]<IStream*, uint>)(lpVtbl[2]))((IStream*)Unsafe.AsPointer(ref this));
        }

        [VtblIndex(3)]
        public int Read()
        {
            return ((delegate* unmanaged[Thiscall]<IStream*, int>)(lpVtbl[3]))((IStream*)Unsafe.AsPointer(ref this));
        }

        [VtblIndex(4)]
        public int Write()
        {
            return ((delegate* unmanaged[Thiscall]<IStream*, int>)(lpVtbl[4]))((IStream*)Unsafe.AsPointer(ref this));
        }

        [VtblIndex(5)]
        public int Seek()
        {
            return ((delegate* unmanaged[Thiscall]<IStream*, int>)(lpVtbl[5]))((IStream*)Unsafe.AsPointer(ref this));
        }

        [VtblIndex(6)]
        public int Clone()
        {
            return ((delegate* unmanaged[Thiscall]<IStream*, int>)(lpVtbl[6]))((IStream*)Unsafe.AsPointer(ref this));
        }
    }

    [NativeTypeName("struct IStreamAsync : IStream")]
    public unsafe partial struct IStreamAsync
    {
        public void** lpVtbl;

        [VtblIndex(0)]
        public int QueryInterface()
        {
            return ((delegate* unmanaged[Thiscall]<IStreamAsync*, int>)(lpVtbl[0]))((IStreamAsync*)Unsafe.AsPointer(ref this));
        }

        [VtblIndex(1)]
        [return: NativeTypeName("unsigned int")]
        public uint AddRef()
        {
            return ((delegate* unmanaged[Thiscall]<IStreamAsync*, uint>)(lpVtbl[1]))((IStreamAsync*)Unsafe.AsPointer(ref this));
        }

        [VtblIndex(2)]
        [return: NativeTypeName("unsigned int")]
        public uint Release()
        {
            return ((delegate* unmanaged[Thiscall]<IStreamAsync*, uint>)(lpVtbl[2]))((IStreamAsync*)Unsafe.AsPointer(ref this));
        }

        [VtblIndex(3)]
        public int Read()
        {
            return ((delegate* unmanaged[Thiscall]<IStreamAsync*, int>)(lpVtbl[3]))((IStreamAsync*)Unsafe.AsPointer(ref this));
        }

        [VtblIndex(4)]
        public int Write()
        {
            return ((delegate* unmanaged[Thiscall]<IStreamAsync*, int>)(lpVtbl[4]))((IStreamAsync*)Unsafe.AsPointer(ref this));
        }

        [VtblIndex(5)]
        public int Seek()
        {
            return ((delegate* unmanaged[Thiscall]<IStreamAsync*, int>)(lpVtbl[5]))((IStreamAsync*)Unsafe.AsPointer(ref this));
        }

        [VtblIndex(6)]
        public int Clone()
        {
            return ((delegate* unmanaged[Thiscall]<IStreamAsync*, int>)(lpVtbl[6]))((IStreamAsync*)Unsafe.AsPointer(ref this));
        }

        [VtblIndex(7)]
        public int ReadAsync()
        {
            return ((delegate* unmanaged[Thiscall]<IStreamAsync*, int>)(lpVtbl[7]))((IStreamAsync*)Unsafe.AsPointer(ref this));
        }
    }
}
