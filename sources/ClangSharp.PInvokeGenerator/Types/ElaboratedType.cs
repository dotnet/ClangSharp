using System.Diagnostics;
using ClangSharp.Interop;

namespace ClangSharp
{
    internal sealed class ElaboratedType : TypeWithKeyword
    {
        public ElaboratedType(CXType handle, TranslationUnit translationUnit) : base(handle, translationUnit)
        {
            Debug.Assert(handle.kind == CXTypeKind.CXType_Elaborated);
            NamedType = TranslationUnit.GetOrCreateType(Handle.NamedType, () => Create(Handle.NamedType, TranslationUnit));
        }

        public Type NamedType { get; }
    }
}
