using System.Runtime.CompilerServices;

namespace ClangSharp.Test
{
    public unsafe partial struct MyStruct
    {
        public Vtbl* lpVtbl;

        public int GetType(int obj)
        {
            return lpVtbl->GetType((MyStruct*)Unsafe.AsPointer(ref this), obj);
        }

        public new int GetType()
        {
            return lpVtbl->GetType1((MyStruct*)Unsafe.AsPointer(ref this));
        }

        public int GetType(int objA, int objB)
        {
            return lpVtbl->GetType2((MyStruct*)Unsafe.AsPointer(ref this), objA, objB);
        }

        public partial struct Vtbl
        {
            [NativeTypeName("int (int)")]
            public new delegate* unmanaged[Thiscall]<MyStruct*, int, int> GetType;

            [NativeTypeName("int ()")]
            public delegate* unmanaged[Thiscall]<MyStruct*, int> GetType1;

            [NativeTypeName("int (int, int)")]
            public delegate* unmanaged[Thiscall]<MyStruct*, int, int, int> GetType2;
        }
    }
}
