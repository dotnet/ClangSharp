using System;

namespace ClangSharp.Interop
{
    public unsafe partial struct CXDiagnostic : IDisposable, IEquatable<CXDiagnostic>
    {
        public CXDiagnostic(IntPtr handle)
        {
            Handle = handle;
        }

        public static CXDiagnosticDisplayOptions DefaultDisplayOptions => (CXDiagnosticDisplayOptions)clang.defaultDiagnosticDisplayOptions();

        public uint Category => clang.getDiagnosticCategory(this);

        public CXString CategoryText => clang.getDiagnosticCategoryText(this);

        public CXDiagnosticSet ChildDiagnostics => (CXDiagnosticSet)clang.getChildDiagnostics(this);

        public IntPtr Handle { get; set; }

        public CXSourceLocation Location => clang.getDiagnosticLocation(this);

        public uint NumFixIts => clang.getDiagnosticNumFixIts(this);

        public uint NumRanges => clang.getDiagnosticNumRanges(this);

        public CXDiagnosticSeverity Severity => clang.getDiagnosticSeverity(this);

        public CXString Spelling => clang.getDiagnosticSpelling(this);

        public static explicit operator CXDiagnostic(void* value) => new CXDiagnostic((IntPtr)value);

        public static implicit operator void*(CXDiagnostic value) => (void*)value.Handle;

        public static bool operator ==(CXDiagnostic left, CXDiagnostic right) => left.Handle == right.Handle;

        public static bool operator !=(CXDiagnostic left, CXDiagnostic right) => left.Handle != right.Handle;

        public void Dispose()
        {
            if (Handle != IntPtr.Zero)
            {
                clang.disposeDiagnostic(this);
                Handle = IntPtr.Zero;
            }
        }

        public override bool Equals(object obj) => (obj is CXDiagnostic other) && Equals(other);

        public bool Equals(CXDiagnostic other) => this == other;

        public CXString Format(CXDiagnosticDisplayOptions options) => clang.formatDiagnostic(this, (uint)options);

        [Obsolete("Use " + nameof(CategoryText) + " instead.")]
        public static CXString GetCategoryName(uint category) => clang.getDiagnosticCategoryName(category);

        public CXString GetFixIt(uint fixIt, out CXSourceRange replacementRange)
        {
            fixed (CXSourceRange* pReplacementRange = &replacementRange)
            {
                return clang.getDiagnosticFixIt(this, fixIt, pReplacementRange);
            }
        }

        public override int GetHashCode() => Handle.GetHashCode();

        public CXString GetOption(out CXString disable)
        {
            fixed (CXString* pDisable = &disable)
            {
                return clang.getDiagnosticOption(this, pDisable);
            }
        }

        public CXSourceRange GetRange(uint range) => clang.getDiagnosticRange(this, range);

        public override string ToString() => Spelling.ToString();
    }
}
