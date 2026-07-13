using System.Runtime.CompilerServices;

namespace ClangSharp.Test
{
    public unsafe partial struct Foo
    {
        public void** lpVtbl;

        public void Dispose()
        {
            ((delegate* unmanaged[Thiscall]<Foo*, void>)(lpVtbl[0]))((Foo*)Unsafe.AsPointer(ref this));
        }
    }

    public unsafe partial struct Bar
    {
        public void** lpVtbl;

        public void Dispose()
        {
            ((delegate* unmanaged[Thiscall]<Bar*, void>)(lpVtbl[0]))((Bar*)Unsafe.AsPointer(ref this));
        }
    }

    [NativeTypeName("struct Baz : Foo, Bar")]
    public unsafe partial struct Baz
    {
        public void** lpVtbl;

        public Bar Base2;

        public void Dispose()
        {
            ((delegate* unmanaged[Thiscall]<Baz*, void>)(lpVtbl[0]))((Baz*)Unsafe.AsPointer(ref this));
        }
    }
}
