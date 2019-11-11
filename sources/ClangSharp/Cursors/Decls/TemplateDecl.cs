// Copyright (c) Microsoft and Contributors. All rights reserved. Licensed under the University of Illinois/NCSA Open Source License. See LICENSE.txt in the project root for license information.

using ClangSharp.Interop;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ClangSharp
{
    public class TemplateDecl : NamedDecl
    {
        private readonly Lazy<IReadOnlyList<NamedDecl>> _templateParameters;

        private protected TemplateDecl(CXCursor handle, CXCursorKind expectedKind) : base(handle, expectedKind)
        {
            _templateParameters = new Lazy<IReadOnlyList<NamedDecl>>(() => CursorChildren.Where((cursor) => (cursor is TemplateTypeParmDecl) || (cursor is NonTypeTemplateParmDecl) || (cursor is TemplateTemplateParmDecl)).Cast<NamedDecl>().ToList());
        }

        public IReadOnlyList<NamedDecl> TemplateParameters => _templateParameters.Value;
    }
}
