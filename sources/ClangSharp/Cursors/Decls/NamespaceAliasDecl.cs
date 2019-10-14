using System;
using System.Collections.Generic;
using System.Linq;
using ClangSharp.Interop;

namespace ClangSharp
{
    public sealed class NamespaceAliasDecl: NamedDecl, IRedeclarable<NamespaceDecl>
    {
        internal NamespaceAliasDecl(CXCursor handle) : base(handle, CXCursorKind.CXCursor_NamespaceAlias)
        {
        }
    }
}
