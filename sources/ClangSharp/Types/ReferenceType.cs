using ClangSharp.Interop;

namespace ClangSharp
{
    public class ReferenceType : Type
    {
        protected ReferenceType(CXType handle, TranslationUnitDecl translationUnit) : base(handle, translationUnit)
        {
            PointeeType = TranslationUnit.GetOrCreateType(Handle.PointeeType, () => Create(Handle.PointeeType, TranslationUnit));
        }

        public Type PointeeType { get; }
    }
}
