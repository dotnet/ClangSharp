using ClangSharp.Interop;

namespace ClangSharp
{
    internal class ArrayType : Type
    {
        protected ArrayType(CXType handle, TranslationUnit translationUnit) : base(handle, translationUnit)
        {
            ElementType = TranslationUnit.GetOrCreateType(Handle.ArrayElementType, () => Create(Handle.ArrayElementType, TranslationUnit));
        }

        public Type ElementType { get; }
    }
}
