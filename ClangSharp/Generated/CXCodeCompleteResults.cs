namespace ClangSharp
{
    using System;
    using System.Runtime.InteropServices;

    public partial struct CXCodeCompleteResults
    {
        public IntPtr Results;
        public uint NumResults;
    }
}
