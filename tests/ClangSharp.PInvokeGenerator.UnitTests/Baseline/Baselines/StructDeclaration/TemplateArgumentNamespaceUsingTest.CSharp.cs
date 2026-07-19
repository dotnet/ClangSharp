using ClangSharp.Test;
using System.Numerics;
using System.Runtime.CompilerServices;

namespace System.Numerics
{
    public partial struct Matrix4x4
    {
        [NativeTypeName("float[16]")]
        public _m_e__FixedBuffer m;

        [InlineArray(16)]
        public partial struct _m_e__FixedBuffer
        {
            public float e0;
        }
    }

    public unsafe partial struct ISpatial : ISpatial.Interface
    {
        public void** lpVtbl;

        [return: NativeTypeName("ABI::Windows::Foundation::IReference<ABI::Windows::Foundation::Numerics::Matrix4x4> *")]
        public IReference<Matrix4x4>* GetTransform()
        {
            return ((delegate* unmanaged[Thiscall]<ISpatial*, IReference<Matrix4x4>*>)(lpVtbl[0]))((ISpatial*)Unsafe.AsPointer(ref this));
        }

        public interface Interface
        {
            [return: NativeTypeName("ABI::Windows::Foundation::IReference<ABI::Windows::Foundation::Numerics::Matrix4x4> *")]
            IReference<Matrix4x4>* GetTransform();
        }
    }
}
