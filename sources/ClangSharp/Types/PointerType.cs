using System;
using ClangSharp.Interop;

namespace ClangSharp
{
    public sealed class PointerType : Type
    {
        private readonly Lazy<Type> _pointeeType;

        internal PointerType(CXType handle) : base(handle, CXTypeKind.CXType_Pointer)
        {
            _pointeeType = new Lazy<Type>(() => TranslationUnit.GetOrCreate<Type>(Handle.PointeeType));
        }

        public Type PointeeType => _pointeeType.Value;
    }
}
