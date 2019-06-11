namespace ClangSharp
{
    public unsafe partial struct CXIdxIBOutletCollectionAttrInfo
    {
        [NativeTypeName("const CXIdxAttrInfo *")]
        public CXIdxAttrInfo* attrInfo;

        [NativeTypeName("const CXIdxEntityInfo *")]
        public CXIdxEntityInfo* objcClass;

        public CXCursor classCursor;

        public CXIdxLoc classLoc;
    }
}
