using ClangSharp.Test;
using System;
using System.Numerics;
using System.Runtime.InteropServices;

namespace System.Numerics
{
    public unsafe partial struct Matrix4x4
    {
        [NativeTypeName("float[16]")]
        public fixed float m[16];
    }

    public unsafe partial struct ISpatial : ISpatial.Interface
    {
        public void** lpVtbl;

        [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
        [return: NativeTypeName("ABI::Windows::Foundation::IReference<ABI::Windows::Foundation::Numerics::Matrix4x4> *")]
        public delegate IReference<Matrix4x4>* _GetTransform(ISpatial* pThis);

        [return: NativeTypeName("ABI::Windows::Foundation::IReference<ABI::Windows::Foundation::Numerics::Matrix4x4> *")]
        public IReference<Matrix4x4>* GetTransform()
        {
            fixed (ISpatial* pThis = &this)
            {
                return Marshal.GetDelegateForFunctionPointer<_GetTransform>((IntPtr)(lpVtbl[0]))(pThis);
            }
        }

        public interface Interface
        {
            [return: NativeTypeName("ABI::Windows::Foundation::IReference<ABI::Windows::Foundation::Numerics::Matrix4x4> *")]
            IReference<Matrix4x4>* GetTransform();
        }
    }
}
