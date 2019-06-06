using System.Diagnostics;

namespace ClangSharp
{
    internal sealed class AttributedType : Type
    {
        public AttributedType(CXType handle, TranslationUnit translationUnit) : base(handle, translationUnit)
        {
            Debug.Assert(handle.kind == CXTypeKind.CXType_Attributed);
            ModifiedType = TranslationUnit.GetOrCreateType(Handle.ModifiedType, () => Create(Handle.ModifiedType, TranslationUnit));
        }

        public Type ModifiedType { get; }
    }
}
