namespace ClangSharp
{
    internal class ReferenceType : Type
    {
        protected ReferenceType(CXType handle, TranslationUnit translationUnit) : base(handle, translationUnit)
        {
            PointeeType = TranslationUnit.GetOrCreateType(Handle.PointeeType, () => Create(Handle.PointeeType, TranslationUnit));
        }

        public Type PointeeType { get; }
    }
}
