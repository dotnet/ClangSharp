using System;
using System.Runtime.InteropServices;

namespace ClangSharp.Test
{
    public unsafe partial struct IUnknown
    {
        public void** lpVtbl;

        [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
        public delegate int _QueryInterface(IUnknown* pThis);

        [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
        [return: NativeTypeName("unsigned int")]
        public delegate uint _AddRef(IUnknown* pThis);

        [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
        [return: NativeTypeName("unsigned int")]
        public delegate uint _Release(IUnknown* pThis);

        [VtblIndex(0)]
        public int QueryInterface()
        {
            fixed (IUnknown* pThis = &this)
            {
                return Marshal.GetDelegateForFunctionPointer<_QueryInterface>((IntPtr)(lpVtbl[0]))(pThis);
            }
        }

        [VtblIndex(1)]
        [return: NativeTypeName("unsigned int")]
        public uint AddRef()
        {
            fixed (IUnknown* pThis = &this)
            {
                return Marshal.GetDelegateForFunctionPointer<_AddRef>((IntPtr)(lpVtbl[1]))(pThis);
            }
        }

        [VtblIndex(2)]
        [return: NativeTypeName("unsigned int")]
        public uint Release()
        {
            fixed (IUnknown* pThis = &this)
            {
                return Marshal.GetDelegateForFunctionPointer<_Release>((IntPtr)(lpVtbl[2]))(pThis);
            }
        }
    }

    [NativeTypeName("struct ISequentialStream : IUnknown")]
    public unsafe partial struct ISequentialStream
    {
        public void** lpVtbl;

        [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
        public delegate int _QueryInterface(ISequentialStream* pThis);

        [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
        [return: NativeTypeName("unsigned int")]
        public delegate uint _AddRef(ISequentialStream* pThis);

        [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
        [return: NativeTypeName("unsigned int")]
        public delegate uint _Release(ISequentialStream* pThis);

        [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
        public delegate int _Read(ISequentialStream* pThis);

        [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
        public delegate int _Write(ISequentialStream* pThis);

        [VtblIndex(0)]
        public int QueryInterface()
        {
            fixed (ISequentialStream* pThis = &this)
            {
                return Marshal.GetDelegateForFunctionPointer<_QueryInterface>((IntPtr)(lpVtbl[0]))(pThis);
            }
        }

        [VtblIndex(1)]
        [return: NativeTypeName("unsigned int")]
        public uint AddRef()
        {
            fixed (ISequentialStream* pThis = &this)
            {
                return Marshal.GetDelegateForFunctionPointer<_AddRef>((IntPtr)(lpVtbl[1]))(pThis);
            }
        }

        [VtblIndex(2)]
        [return: NativeTypeName("unsigned int")]
        public uint Release()
        {
            fixed (ISequentialStream* pThis = &this)
            {
                return Marshal.GetDelegateForFunctionPointer<_Release>((IntPtr)(lpVtbl[2]))(pThis);
            }
        }

        [VtblIndex(3)]
        public int Read()
        {
            fixed (ISequentialStream* pThis = &this)
            {
                return Marshal.GetDelegateForFunctionPointer<_Read>((IntPtr)(lpVtbl[3]))(pThis);
            }
        }

        [VtblIndex(4)]
        public int Write()
        {
            fixed (ISequentialStream* pThis = &this)
            {
                return Marshal.GetDelegateForFunctionPointer<_Write>((IntPtr)(lpVtbl[4]))(pThis);
            }
        }
    }

    [NativeTypeName("struct IStream : ISequentialStream")]
    public unsafe partial struct IStream
    {
        public void** lpVtbl;

        [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
        public delegate int _QueryInterface(IStream* pThis);

        [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
        [return: NativeTypeName("unsigned int")]
        public delegate uint _AddRef(IStream* pThis);

        [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
        [return: NativeTypeName("unsigned int")]
        public delegate uint _Release(IStream* pThis);

        [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
        public delegate int _Read(IStream* pThis);

        [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
        public delegate int _Write(IStream* pThis);

        [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
        public delegate int _Seek(IStream* pThis);

        [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
        public delegate int _Clone(IStream* pThis);

        [VtblIndex(0)]
        public int QueryInterface()
        {
            fixed (IStream* pThis = &this)
            {
                return Marshal.GetDelegateForFunctionPointer<_QueryInterface>((IntPtr)(lpVtbl[0]))(pThis);
            }
        }

        [VtblIndex(1)]
        [return: NativeTypeName("unsigned int")]
        public uint AddRef()
        {
            fixed (IStream* pThis = &this)
            {
                return Marshal.GetDelegateForFunctionPointer<_AddRef>((IntPtr)(lpVtbl[1]))(pThis);
            }
        }

        [VtblIndex(2)]
        [return: NativeTypeName("unsigned int")]
        public uint Release()
        {
            fixed (IStream* pThis = &this)
            {
                return Marshal.GetDelegateForFunctionPointer<_Release>((IntPtr)(lpVtbl[2]))(pThis);
            }
        }

        [VtblIndex(3)]
        public int Read()
        {
            fixed (IStream* pThis = &this)
            {
                return Marshal.GetDelegateForFunctionPointer<_Read>((IntPtr)(lpVtbl[3]))(pThis);
            }
        }

        [VtblIndex(4)]
        public int Write()
        {
            fixed (IStream* pThis = &this)
            {
                return Marshal.GetDelegateForFunctionPointer<_Write>((IntPtr)(lpVtbl[4]))(pThis);
            }
        }

        [VtblIndex(5)]
        public int Seek()
        {
            fixed (IStream* pThis = &this)
            {
                return Marshal.GetDelegateForFunctionPointer<_Seek>((IntPtr)(lpVtbl[5]))(pThis);
            }
        }

        [VtblIndex(6)]
        public int Clone()
        {
            fixed (IStream* pThis = &this)
            {
                return Marshal.GetDelegateForFunctionPointer<_Clone>((IntPtr)(lpVtbl[6]))(pThis);
            }
        }
    }

    [NativeTypeName("struct IStreamAsync : IStream")]
    public unsafe partial struct IStreamAsync
    {
        public void** lpVtbl;

        [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
        public delegate int _QueryInterface(IStreamAsync* pThis);

        [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
        [return: NativeTypeName("unsigned int")]
        public delegate uint _AddRef(IStreamAsync* pThis);

        [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
        [return: NativeTypeName("unsigned int")]
        public delegate uint _Release(IStreamAsync* pThis);

        [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
        public delegate int _Read(IStreamAsync* pThis);

        [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
        public delegate int _Write(IStreamAsync* pThis);

        [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
        public delegate int _Seek(IStreamAsync* pThis);

        [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
        public delegate int _Clone(IStreamAsync* pThis);

        [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
        public delegate int _ReadAsync(IStreamAsync* pThis);

        [VtblIndex(0)]
        public int QueryInterface()
        {
            fixed (IStreamAsync* pThis = &this)
            {
                return Marshal.GetDelegateForFunctionPointer<_QueryInterface>((IntPtr)(lpVtbl[0]))(pThis);
            }
        }

        [VtblIndex(1)]
        [return: NativeTypeName("unsigned int")]
        public uint AddRef()
        {
            fixed (IStreamAsync* pThis = &this)
            {
                return Marshal.GetDelegateForFunctionPointer<_AddRef>((IntPtr)(lpVtbl[1]))(pThis);
            }
        }

        [VtblIndex(2)]
        [return: NativeTypeName("unsigned int")]
        public uint Release()
        {
            fixed (IStreamAsync* pThis = &this)
            {
                return Marshal.GetDelegateForFunctionPointer<_Release>((IntPtr)(lpVtbl[2]))(pThis);
            }
        }

        [VtblIndex(3)]
        public int Read()
        {
            fixed (IStreamAsync* pThis = &this)
            {
                return Marshal.GetDelegateForFunctionPointer<_Read>((IntPtr)(lpVtbl[3]))(pThis);
            }
        }

        [VtblIndex(4)]
        public int Write()
        {
            fixed (IStreamAsync* pThis = &this)
            {
                return Marshal.GetDelegateForFunctionPointer<_Write>((IntPtr)(lpVtbl[4]))(pThis);
            }
        }

        [VtblIndex(5)]
        public int Seek()
        {
            fixed (IStreamAsync* pThis = &this)
            {
                return Marshal.GetDelegateForFunctionPointer<_Seek>((IntPtr)(lpVtbl[5]))(pThis);
            }
        }

        [VtblIndex(6)]
        public int Clone()
        {
            fixed (IStreamAsync* pThis = &this)
            {
                return Marshal.GetDelegateForFunctionPointer<_Clone>((IntPtr)(lpVtbl[6]))(pThis);
            }
        }

        [VtblIndex(7)]
        public int ReadAsync()
        {
            fixed (IStreamAsync* pThis = &this)
            {
                return Marshal.GetDelegateForFunctionPointer<_ReadAsync>((IntPtr)(lpVtbl[7]))(pThis);
            }
        }
    }
}
