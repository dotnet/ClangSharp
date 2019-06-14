using System.Collections.Generic;
using System.Diagnostics;
using ClangSharp.Interop;

namespace ClangSharp
{
    public sealed class FunctionProtoType : FunctionType
    {
        private readonly Type[] _parameters;

        public FunctionProtoType(CXType handle, TranslationUnit translationUnit) : base(handle, translationUnit)
        {
            Debug.Assert(handle.kind == CXTypeKind.CXType_FunctionProto);

            _parameters = new Type[Handle.NumArgTypes];

            for (uint index = 0; index < Handle.NumArgTypes; index++)
            {
                var parameterTypeHandle = Handle.GetArgType(index);
                var parameterType = TranslationUnit.GetOrCreateType(parameterTypeHandle, () => Create(parameterTypeHandle, TranslationUnit));
                _parameters[index] = parameterType;
            }
        }

        public CXCursor_ExceptionSpecificationKind ExceptionSpecType => Handle.ExceptionSpecificationType;

        public bool IsVariadic => Handle.IsFunctionTypeVariadic;

        public IReadOnlyList<Type> Parameters => _parameters;

        public CXRefQualifierKind RefQualifier => Handle.CXXRefQualifier;
    }
}
