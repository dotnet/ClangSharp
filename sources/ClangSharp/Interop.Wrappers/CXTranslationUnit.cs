using System;

namespace ClangSharp.Interop
{
    public unsafe partial struct CXTranslationUnit : IEquatable<CXTranslationUnit>
    {
        public CXTranslationUnit(IntPtr handle)
        {
            Handle = handle;
        }

        public IntPtr Handle { get; set; }

        public static implicit operator CXTranslationUnit(CXTranslationUnitImpl* value) => new CXTranslationUnit((IntPtr)value);

        public static implicit operator CXTranslationUnitImpl*(CXTranslationUnit value) => (CXTranslationUnitImpl*)value.Handle;

        public static bool operator ==(CXTranslationUnit left, CXTranslationUnit right) => left.Handle == right.Handle;

        public static bool operator !=(CXTranslationUnit left, CXTranslationUnit right) => left.Handle != right.Handle;

        public override bool Equals(object obj) => (obj is CXTranslationUnit other) && Equals(other);

        public bool Equals(CXTranslationUnit other) => (this == other);

        public override int GetHashCode() => Handle.GetHashCode();
    }
}
