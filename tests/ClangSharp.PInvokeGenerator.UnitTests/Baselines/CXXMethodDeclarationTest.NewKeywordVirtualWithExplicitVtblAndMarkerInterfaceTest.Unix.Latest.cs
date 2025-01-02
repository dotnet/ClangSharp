using System.Runtime.CompilerServices;

namespace ClangSharp.Test
{
    public unsafe partial struct MyStruct : MyStruct.Interface
    {
        public Vtbl<MyStruct>* lpVtbl;

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

        public interface Interface
        {
            int GetType(int obj);

            int GetType();

            int GetType(int objA, int objB);
        }

        public partial struct Vtbl<TSelf>
            where TSelf : unmanaged, Interface
        {
            [NativeTypeName("int (int)")]
            public new delegate* unmanaged[Thiscall]<TSelf*, int, int> GetType;

            [NativeTypeName("int ()")]
            public delegate* unmanaged[Thiscall]<TSelf*, int> GetType1;

            [NativeTypeName("int (int, int)")]
            public delegate* unmanaged[Thiscall]<TSelf*, int, int, int> GetType2;
        }
    }
}
