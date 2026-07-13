using System.Runtime.CompilerServices;

namespace ClangSharp.Test
{
    public unsafe partial struct Foo
    {
        public void** lpVtbl;

        public int x;

        public void A()
        {
            ((delegate* unmanaged[Thiscall]<Foo*, void>)(lpVtbl[0]))((Foo*)Unsafe.AsPointer(ref this));
        }
    }

    public unsafe partial struct Bar
    {
        public void** lpVtbl;

        public void B()
        {
            ((delegate* unmanaged[Thiscall]<Bar*, void>)(lpVtbl[0]))((Bar*)Unsafe.AsPointer(ref this));
        }
    }

    [NativeTypeName("struct Baz : Foo, Bar")]
    public partial struct Baz
    {
        public Foo Base1;

        public Bar Base2;
    }
}
