using System;

namespace ClangSharp
{
    public partial struct CXDiagnostic : IDisposable
    {
        public static CXDiagnosticDisplayOptions DefaultDisplayOptions => (CXDiagnosticDisplayOptions)clang.defaultDiagnosticDisplayOptions();

        public uint Category => clang.getDiagnosticCategory(this);

        public CXString CategoryText => clang.getDiagnosticCategoryText(this);

        public CXDiagnosticSet ChildDiagnostics => clang.getChildDiagnostics(this);

        public CXSourceLocation Location => clang.getDiagnosticLocation(this);

        public uint NumFixIts => clang.getDiagnosticNumFixIts(this);

        public uint NumRanges => clang.getDiagnosticNumRanges(this);

        public CXDiagnosticSeverity Severity => clang.getDiagnosticSeverity(this);

        public CXString Spelling => clang.getDiagnosticSpelling(this);

        public void Dispose() => clang.disposeDiagnostic(this);

        public CXString Format(CXDiagnosticDisplayOptions options) => clang.formatDiagnostic(this, (uint)options);

        [Obsolete("Use " + nameof(CategoryText) + " instead.")]
        public static CXString GetCategoryName(uint category) => clang.getDiagnosticCategoryName(category);

        public CXString GetFixIt(uint fixIt, out CXSourceRange replacementRange) => clang.getDiagnosticFixIt(this, fixIt, out replacementRange);

        public CXString GetOption(out CXString disable) => clang.getDiagnosticOption(this, out disable);

        public CXSourceRange GetRange(uint range) => clang.getDiagnosticRange(this, range);

        public override string ToString() => Spelling.ToString();
    }
}
