using System;
using ClangSharp.Interop;

namespace ClangSharp
{
    public class ReferenceType : Type
    {
        private readonly Lazy<Type> _pointeeType;

        private protected ReferenceType(CXType handle, CXTypeKind expectedKind) : base(handle, expectedKind)
        {
            _pointeeType = new Lazy<Type>(() => TranslationUnit.GetOrCreate<Type>(Handle.PointeeType));
        }

        public Type PointeeType => _pointeeType.Value;
    }
}
