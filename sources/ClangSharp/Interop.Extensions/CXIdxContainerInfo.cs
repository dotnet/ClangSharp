namespace ClangSharp.Interop
{
    public unsafe partial struct CXIdxContainerInfo
    {
        public CXIdxClientContainer ClientContainer
        {
            get
            {
                fixed (CXIdxContainerInfo* pThis = &this)
                {
                    return (CXIdxClientContainer)clang.index_getClientContainer(pThis);
                }
            }

            set
            {
                fixed (CXIdxContainerInfo* pThis = &this)
                {
                    clang.index_setClientContainer(pThis, value);
                }
            }
        }
    }
}
