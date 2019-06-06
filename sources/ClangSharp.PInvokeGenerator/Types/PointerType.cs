using System.Diagnostics;

namespace ClangSharp
{
    internal sealed class PointerType : Type
    {
        public PointerType(CXType handle, TranslationUnit translationUnit) : base(handle, translationUnit)
        {
            Debug.Assert(handle.kind == CXTypeKind.CXType_Pointer);
            PointeeType = TranslationUnit.GetOrCreateType(Handle.PointeeType, () => Create(Handle.PointeeType, TranslationUnit));
        }

        public Type PointeeType { get; }
    }
}
