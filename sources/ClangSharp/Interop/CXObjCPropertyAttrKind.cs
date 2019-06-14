namespace ClangSharp.Interop
{
    public enum CXObjCPropertyAttrKind
    {
        CXObjCPropertyAttr_noattr = 0x00,
        CXObjCPropertyAttr_readonly = 0x01,
        CXObjCPropertyAttr_getter = 0x02,
        CXObjCPropertyAttr_assign = 0x04,
        CXObjCPropertyAttr_readwrite = 0x08,
        CXObjCPropertyAttr_retain = 0x10,
        CXObjCPropertyAttr_copy = 0x20,
        CXObjCPropertyAttr_nonatomic = 0x40,
        CXObjCPropertyAttr_setter = 0x80,
        CXObjCPropertyAttr_atomic = 0x100,
        CXObjCPropertyAttr_weak = 0x200,
        CXObjCPropertyAttr_strong = 0x400,
        CXObjCPropertyAttr_unsafe_unretained = 0x800,
        CXObjCPropertyAttr_class = 0x1000,
    }
}
