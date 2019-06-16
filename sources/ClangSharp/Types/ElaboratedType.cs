using System;
using ClangSharp.Interop;

namespace ClangSharp
{
    public sealed class ElaboratedType : TypeWithKeyword
    {
        private readonly Lazy<Type> _namedType;

        internal ElaboratedType(CXType handle) : base(handle, CXTypeKind.CXType_Elaborated)
        {
            _namedType = new Lazy<Type>(() => TranslationUnit.GetOrCreate<Type>(Handle.NamedType));
        }

        public Type NamedType => _namedType.Value;
    }
}
