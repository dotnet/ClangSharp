using System.Collections.Generic;
using System.Diagnostics;
using ClangSharp.Interop;

namespace ClangSharp
{
    internal sealed class TypedefDecl : TypedefNameDecl
    {
        private readonly List<ParmVarDecl> _parameters = new List<ParmVarDecl>();

        public TypedefDecl(CXCursor handle, Cursor parent) : base(handle, parent)
        {
            Debug.Assert(handle.Kind == CXCursorKind.CXCursor_TypedefDecl);
        }

        public IReadOnlyList<ParmVarDecl> Parameters => _parameters;

        protected override Decl GetOrAddDecl(CXCursor childHandle)
        {
            var decl = base.GetOrAddDecl(childHandle);

            if (decl is ParmVarDecl parmVarDecl)
            {
                _parameters.Add(parmVarDecl);
            }

            return decl;
        }
    }
}
