using System;
using ClangSharp.Interop;

namespace ClangSharp
{
    public class FunctionType : Type
    {
        private readonly Lazy<Type> _returnType;

        private protected FunctionType(CXType handle, CXTypeKind expectedKind) : base(handle, expectedKind)
        {
            _returnType = new Lazy<Type>(() => TranslationUnit.GetOrCreate<Type>(Handle.ResultType));
        }

        public CXCallingConv CallConv => Handle.FunctionTypeCallingConv;

        public Type ReturnType => _returnType.Value;
    }
}
