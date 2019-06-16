using System;
using System.Diagnostics;
using ClangSharp.Interop;

namespace ClangSharp
{
    public class Ref : Cursor
    {
        private readonly Lazy<Type> _type;

        private protected Ref(CXCursor handle, CXCursorKind expectedKind) : base(handle, expectedKind)
        {
            _type = new Lazy<Type>(() => TranslationUnit.GetOrCreate<Type>(Handle.Type));
        }

        public Type Type => _type.Value;

        internal static new Ref Create(CXCursor handle)
        {
            Ref result;

            switch (handle.Kind)
            {

                case CXCursorKind.CXCursor_CXXBaseSpecifier:
                {
                    result = new CXXBaseSpecifier(handle);
                    break;
                }

                default:
                {
                    // Debug.WriteLine($"Unhandled reference kind: {handle.KindSpelling}.");
                    // Debugger.Break();

                    result = new Ref(handle, handle.Kind);
                    break;
                }
            }

            return result;
        }
    }
}
