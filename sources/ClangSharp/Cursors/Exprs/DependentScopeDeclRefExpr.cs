// Copyright (c) Microsoft and Contributors. All rights reserved. Licensed under the University of Illinois/NCSA Open Source License. See LICENSE.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Diagnostics;
using ClangSharp.Interop;

namespace ClangSharp
{
    public sealed class DependentScopeDeclRefExpr : Expr
    {
        private readonly Lazy<IReadOnlyList<TemplateArgumentLoc>> _templateArgs;

        internal DependentScopeDeclRefExpr(CXCursor handle) : base(handle, CXCursorKind.CXCursor_DeclRefExpr, CX_StmtClass.CX_StmtClass_DependentScopeDeclRefExpr)
        {
            Debug.Assert(NumChildren is 0);

            _templateArgs = new Lazy<IReadOnlyList<TemplateArgumentLoc>>(() => {
                var numTemplateArgs = Handle.NumTemplateArguments;
                var templateArgs = new List<TemplateArgumentLoc>(numTemplateArgs);

                for (var i = 0; i < numTemplateArgs; i++)
                {
                    var templateArg = TranslationUnit.GetOrCreate(Handle.GetTemplateArgumentLoc(unchecked((uint)i)));
                    templateArgs.Add(templateArg);
                }

                return templateArgs;
            });
        }

        public string DeclName => Handle.Name.CString;

        public bool HasExplicitTemplateArgs => Handle.HasExplicitTemplateArgs;

        public bool HasTemplateKeyword => Handle.HasTemplateKeyword;

        public IReadOnlyList<TemplateArgumentLoc> TemplateArgs => _templateArgs.Value;
    }
}
