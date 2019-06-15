using ClangSharp.Interop;

namespace ClangSharp
{
    public class FunctionType : Type
    {
        protected FunctionType(CXType handle, TranslationUnitDecl translationUnit) : base(handle, translationUnit)
        {
            ReturnType = TranslationUnit.GetOrCreateType(Handle.ResultType, () => Create(Handle.ResultType, TranslationUnit));
        }

        public CXCallingConv CallConv => Handle.FunctionTypeCallingConv;

        public Type ReturnType { get; }
    }
}
