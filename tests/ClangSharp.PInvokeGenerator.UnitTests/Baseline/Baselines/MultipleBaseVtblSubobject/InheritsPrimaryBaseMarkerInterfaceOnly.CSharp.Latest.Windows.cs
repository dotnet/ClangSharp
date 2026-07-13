using System.Runtime.CompilerServices;

namespace ClangSharp.Test
{
    public unsafe partial struct Foo : Foo.Interface
    {
        public Vtbl<Foo>* lpVtbl;

        public void FooMethod()
        {
            lpVtbl->FooMethod((Foo*)Unsafe.AsPointer(ref this));
        }

        public interface Interface
        {
            void FooMethod();
        }

        public partial struct Vtbl<TSelf>
            where TSelf : unmanaged, Interface
        {
            [NativeTypeName("void ()")]
            public delegate* unmanaged[Thiscall]<TSelf*, void> FooMethod;
        }
    }

    public unsafe partial struct Bar : Bar.Interface
    {
        public Vtbl<Bar>* lpVtbl;

        public void BarMethod()
        {
            lpVtbl->BarMethod((Bar*)Unsafe.AsPointer(ref this));
        }

        public interface Interface
        {
            void BarMethod();
        }

        public partial struct Vtbl<TSelf>
            where TSelf : unmanaged, Interface
        {
            [NativeTypeName("void ()")]
            public delegate* unmanaged[Thiscall]<TSelf*, void> BarMethod;
        }
    }

    [NativeTypeName("struct Baz : Foo, Bar")]
    public unsafe partial struct Baz : Baz.Interface
    {
        public Vtbl<Baz>* lpVtbl;

        public Bar Base2;

        public void FooMethod()
        {
            lpVtbl->FooMethod((Baz*)Unsafe.AsPointer(ref this));
        }

        public void BazMethod()
        {
            lpVtbl->BazMethod((Baz*)Unsafe.AsPointer(ref this));
        }

        public interface Interface : Foo.Interface
        {
            void BazMethod();
        }

        public partial struct Vtbl<TSelf>
            where TSelf : unmanaged, Interface
        {
            [NativeTypeName("void ()")]
            public delegate* unmanaged[Thiscall]<TSelf*, void> FooMethod;

            [NativeTypeName("void ()")]
            public delegate* unmanaged[Thiscall]<TSelf*, void> BazMethod;
        }
    }
}
