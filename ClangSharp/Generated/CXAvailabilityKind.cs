namespace ClangSharp
{
    using System;
    using System.Runtime.InteropServices;

    public enum CXAvailabilityKind
    {
        CXAvailability_Available = 0,
        CXAvailability_Deprecated = 1,
        CXAvailability_NotAvailable = 2,
        CXAvailability_NotAccessible = 3,
    }
}
