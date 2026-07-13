using System.Runtime.CompilerServices;

namespace ClangSharp.Test
{
    public unsafe partial struct MyStruct : MyStruct.Interface
    {
        public Vtbl<MyStruct>* lpVtbl;

        public int GetType(int objA, int objB)
        {
            return lpVtbl->GetType((MyStruct*)Unsafe.AsPointer(ref this), objA, objB);
        }

        public new int GetType()
        {
            return lpVtbl->GetType1((MyStruct*)Unsafe.AsPointer(ref this));
        }

        public int GetType(int obj)
        {
            return lpVtbl->GetType2((MyStruct*)Unsafe.AsPointer(ref this), obj);
        }

        public interface Interface
        {
            int GetType(int objA, int objB);

            int GetType();

            int GetType(int obj);
        }

        public partial struct Vtbl<TSelf>
            where TSelf : unmanaged, Interface
        {
            [NativeTypeName("int (int, int)")]
            public new delegate* unmanaged[Thiscall]<TSelf*, int, int, int> GetType;

            [NativeTypeName("int ()")]
            public delegate* unmanaged[Thiscall]<TSelf*, int> GetType1;

            [NativeTypeName("int (int)")]
            public delegate* unmanaged[Thiscall]<TSelf*, int, int> GetType2;
        }
    }
}
