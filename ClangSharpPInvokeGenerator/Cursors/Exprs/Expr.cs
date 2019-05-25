using System;
using System.Diagnostics;
using ClangSharp;

namespace ClangSharpPInvokeGenerator
{
    internal class Expr : Cursor
    {
        private readonly Lazy<Type> _type;

        protected Expr(CXCursor handle, Cursor parent) : base(handle, parent)
        {
            Debug.Assert(handle.IsExpression);
            _type = new Lazy<Type>(() => TranslationUnit.GetOrCreateType(Handle.Type, () => Type.Create(Handle.Type, TranslationUnit)));
        }

        public Type Type => _type.Value;

        protected override void ValidateVisit(ref CXCursor handle)
        {
            // Clang currently uses the PostChildrenVisitor which clears data0

            var modifiedHandle = Handle;
            modifiedHandle.data0 = IntPtr.Zero;

            Debug.Assert(handle.Equals(modifiedHandle));
            handle = Handle;
        }
    }
}
