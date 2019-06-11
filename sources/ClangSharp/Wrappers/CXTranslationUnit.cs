using System;

namespace ClangSharp
{
    public unsafe partial struct CXTranslationUnit
    {
        public CXTranslationUnit(IntPtr pointer)
        {
            Pointer = pointer;
        }

        public IntPtr Pointer;

        public static implicit operator CXTranslationUnit(CXTranslationUnitImpl* value)
        {
            return new CXTranslationUnit((IntPtr)value);
        }

        public static implicit operator CXTranslationUnitImpl*(CXTranslationUnit value)
        {
            return (CXTranslationUnitImpl*)value.Pointer;
        }
    }
}
