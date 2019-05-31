using System;
using System.Diagnostics;

namespace ClangSharp
{
    internal sealed class FunctionTemplateDecl : RedeclarableTemplateDecl
    {
        private readonly Lazy<Type> _type;

        public FunctionTemplateDecl(CXCursor handle, Cursor parent) : base(handle, parent)
        {
            Debug.Assert(handle.Kind == CXCursorKind.CXCursor_FunctionTemplate);
            _type = new Lazy<Type>(() => TranslationUnit.GetOrCreateType(Handle.Type, () => Type.Create(Handle.Type, TranslationUnit)));
        }

        public Type Type => _type.Value;
    }
}
