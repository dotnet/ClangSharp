using ClangSharp.Interop;

namespace ClangSharp
{
    public class ArrayType : Type
    {
        protected ArrayType(CXType handle, TranslationUnitDecl translationUnit) : base(handle, translationUnit)
        {
            ElementType = TranslationUnit.GetOrCreateType(Handle.ArrayElementType, () => Create(Handle.ArrayElementType, TranslationUnit));
        }

        public Type ElementType { get; }
    }
}
