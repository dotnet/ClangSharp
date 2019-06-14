namespace ClangSharp.Interop
{
    public unsafe partial struct CXIdxAttrInfo
    {
        public CXIdxIBOutletCollectionAttrInfo* IBOutletCollectionAttrInfo
        {
            get
            {
                fixed (CXIdxAttrInfo* pThis = &this)
                {
                    return clang.index_getIBOutletCollectionAttrInfo(pThis);
                }
            }
        }
    }
}
