using System;

namespace ClangSharp.Interop
{
    public unsafe partial struct CXDiagnostic : IDisposable
    {
        public static CXDiagnosticDisplayOptions DefaultDisplayOptions => (CXDiagnosticDisplayOptions)clang.defaultDiagnosticDisplayOptions();

        public uint Category => clang.getDiagnosticCategory(this);

        public CXString CategoryText => clang.getDiagnosticCategoryText(this);

        public CXDiagnosticSet ChildDiagnostics => (CXDiagnosticSet)clang.getChildDiagnostics(this);

        public CXSourceLocation Location => clang.getDiagnosticLocation(this);

        public uint NumFixIts => clang.getDiagnosticNumFixIts(this);

        public uint NumRanges => clang.getDiagnosticNumRanges(this);

        public CXDiagnosticSeverity Severity => clang.getDiagnosticSeverity(this);

        public CXString Spelling => clang.getDiagnosticSpelling(this);

        public void Dispose()
        {
            if (Pointer != IntPtr.Zero)
            {
                clang.disposeDiagnostic(this);
                Pointer = IntPtr.Zero;
            }
        }

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
