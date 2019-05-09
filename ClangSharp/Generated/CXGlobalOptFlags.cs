namespace ClangSharp
{
    using System;
    using System.Runtime.InteropServices;

    public enum CXGlobalOptFlags
    {
        CXGlobalOpt_None = 0,
        CXGlobalOpt_ThreadBackgroundPriorityForIndexing = 1,
        CXGlobalOpt_ThreadBackgroundPriorityForEditing = 2,
        CXGlobalOpt_ThreadBackgroundPriorityForAll = 3,
    }
}
