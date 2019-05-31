﻿using System.Diagnostics;

namespace ClangSharp
{
    internal sealed class TemplateTemplateParmDecl : TemplateDecl
    {
        public TemplateTemplateParmDecl(CXCursor handle, Cursor parent) : base(handle, parent)
        {
            Debug.Assert(handle.Kind == CXCursorKind.CXCursor_TemplateTemplateParameter);
        }
    }
}
