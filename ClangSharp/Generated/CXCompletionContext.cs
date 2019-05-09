namespace ClangSharp
{
    public enum CXCompletionContext
    {
        CXCompletionContext_Unexposed = 0,
        CXCompletionContext_AnyType = 1,
        CXCompletionContext_AnyValue = 2,
        CXCompletionContext_ObjCObjectValue = 4,
        CXCompletionContext_ObjCSelectorValue = 8,
        CXCompletionContext_CXXClassTypeValue = 16,
        CXCompletionContext_DotMemberAccess = 32,
        CXCompletionContext_ArrowMemberAccess = 64,
        CXCompletionContext_ObjCPropertyAccess = 128,
        CXCompletionContext_EnumTag = 256,
        CXCompletionContext_UnionTag = 512,
        CXCompletionContext_StructTag = 1024,
        CXCompletionContext_ClassTag = 2048,
        CXCompletionContext_Namespace = 4096,
        CXCompletionContext_NestedNameSpecifier = 8192,
        CXCompletionContext_ObjCInterface = 16384,
        CXCompletionContext_ObjCProtocol = 32768,
        CXCompletionContext_ObjCCategory = 65536,
        CXCompletionContext_ObjCInstanceMessage = 131072,
        CXCompletionContext_ObjCClassMessage = 262144,
        CXCompletionContext_ObjCSelectorName = 524288,
        CXCompletionContext_MacroName = 1048576,
        CXCompletionContext_NaturalLanguage = 2097152,
        CXCompletionContext_IncludedFile = 4194304,
        CXCompletionContext_Unknown = 8388607,
    }
}
