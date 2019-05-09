namespace ClangSharp
{
    using System;
    using System.Runtime.InteropServices;

    public enum CXObjCPropertyAttrKind
    {
        CXObjCPropertyAttr_noattr = 0,
        CXObjCPropertyAttr_readonly = 1,
        CXObjCPropertyAttr_getter = 2,
        CXObjCPropertyAttr_assign = 4,
        CXObjCPropertyAttr_readwrite = 8,
        CXObjCPropertyAttr_retain = 16,
        CXObjCPropertyAttr_copy = 32,
        CXObjCPropertyAttr_nonatomic = 64,
        CXObjCPropertyAttr_setter = 128,
        CXObjCPropertyAttr_atomic = 256,
        CXObjCPropertyAttr_weak = 512,
        CXObjCPropertyAttr_strong = 1024,
        CXObjCPropertyAttr_unsafe_unretained = 2048,
        CXObjCPropertyAttr_class = 4096,
    }
}
