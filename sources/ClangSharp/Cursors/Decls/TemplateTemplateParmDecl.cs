// Copyright (c) Microsoft and Contributors. All rights reserved. Licensed under the University of Illinois/NCSA Open Source License. See LICENSE.txt in the project root for license information.

using ClangSharp.Interop;

namespace ClangSharp
{
    public sealed class TemplateTemplateParmDecl : TemplateDecl, ITemplateParmPosition
    {
        private readonly TemplateArgumentLoc _defaultArgument;

        internal TemplateTemplateParmDecl(CXCursor handle) : base(handle, CXCursorKind.CXCursor_TemplateTemplateParameter, CX_DeclKind.CX_DeclKind_TemplateTemplateParm)
        {
            _defaultArgument = new TemplateArgumentLoc(this, 0);
        }

        public TemplateArgumentLoc DefaultArgument => _defaultArgument;

        public bool DefaultArgumentWasInherited => Handle.HasInheritedDefaultArg;

        public bool IsExpandedParameterPack => Handle.IsExpandedParameterPack;

        public bool IsPackExpansion => Handle.IsPackExpansion;

        public bool IsParameterPack => Handle.IsParameterPack;
    }
}
