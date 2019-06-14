using System;

namespace ClangSharp.Interop
{
    public unsafe partial struct CXModule : IEquatable<CXModule>
    {
        public CXModule(IntPtr handle)
        {
            Handle = handle;
        }

        public CXFile AstFile => (CXFile)clang.Module_getASTFile(this);

        public CXString FullName => clang.Module_getFullName(this);

        public IntPtr Handle { get; set; }

        public bool IsSystem => clang.Module_isSystem(this) != 0;

        public CXString Name => clang.Module_getName(this);

        public CXModule Parent => (CXModule)clang.Module_getParent(this);

        public static explicit operator CXModule(void* value) => new CXModule((IntPtr)value);

        public static implicit operator void*(CXModule value) => (void*)value.Handle;

        public static bool operator ==(CXModule left, CXModule right) => left.Handle == right.Handle;

        public static bool operator !=(CXModule left, CXModule right) => left.Handle != right.Handle;

        public override bool Equals(object obj) => (obj is CXModule other) && Equals(other);

        public bool Equals(CXModule other) => this == other;

        public override int GetHashCode() => Handle.GetHashCode();

        public uint GetNumTopLevelHeaders(CXTranslationUnit translationUnit) => clang.Module_getNumTopLevelHeaders(translationUnit, this);

        public CXFile GetTopLevelHeader(CXTranslationUnit translationUnit, uint index) => (CXFile)clang.Module_getTopLevelHeader(translationUnit, this, index);

        public override string ToString() => FullName.ToString();
    }
}
