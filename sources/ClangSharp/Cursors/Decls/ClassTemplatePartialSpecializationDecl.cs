// Copyright (c) Microsoft and Contributors. All rights reserved. Licensed under the University of Illinois/NCSA Open Source License. See LICENSE.txt in the project root for license information.

using ClangSharp.Interop;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ClangSharp
{
    public sealed class ClassTemplatePartialSpecializationDecl : ClassTemplateSpecializationDecl
    {
        private readonly Lazy<IReadOnlyList<NamedDecl>> _templateParameters;

        internal ClassTemplatePartialSpecializationDecl(CXCursor handle) : base(handle, CXCursorKind.CXCursor_ClassTemplatePartialSpecialization)
        {
            _templateParameters = new Lazy<IReadOnlyList<NamedDecl>>(() => CursorChildren.Where((cursor) => (cursor is TemplateTypeParmDecl) || (cursor is NonTypeTemplateParmDecl) || (cursor is TemplateTemplateParmDecl)).Cast<NamedDecl>().ToList());
        }

        public IReadOnlyList<NamedDecl> TemplateParameters => _templateParameters.Value;
    }
}
