using System;
using ClangSharp.Interop;

namespace ClangSharp
{
    public class ArrayType : Type
    {
        private readonly Lazy<Type> _elementType;

        private protected ArrayType(CXType handle, CXTypeKind expectedKind) : base(handle, expectedKind)
        {
            _elementType = new Lazy<Type>(() => TranslationUnit.GetOrCreate<Type>(Handle.ArrayElementType));
        }

        public Type ElementType => _elementType.Value;
    }
}
