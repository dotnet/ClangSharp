namespace ClangSharp
{
    using System;
    using System.Runtime.InteropServices;

    public partial struct CXPlatformAvailability
    {
        public CXString Platform;
        public CXVersion Introduced;
        public CXVersion Deprecated;
        public CXVersion Obsoleted;
        public int Unavailable;
        public CXString Message;
    }
}
