namespace ClangSharp
{
    public enum CXGlobalOptFlags
    {
        CXGlobalOpt_None = 0x0,
        CXGlobalOpt_ThreadBackgroundPriorityForIndexing = 0x1,
        CXGlobalOpt_ThreadBackgroundPriorityForEditing = 0x2,
        CXGlobalOpt_ThreadBackgroundPriorityForAll = CXGlobalOpt_ThreadBackgroundPriorityForIndexing | CXGlobalOpt_ThreadBackgroundPriorityForEditing,
    }
}
