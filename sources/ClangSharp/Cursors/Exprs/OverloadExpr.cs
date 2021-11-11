// Copyright (c) .NET Foundation and Contributors. All rights reserved. Licensed under the University of Illinois/NCSA Open Source License. See LICENSE.txt in the project root for license information.

using System;
using System.Collections.Generic;
using ClangSharp.Interop;

namespace ClangSharp
{
    public class OverloadExpr : Expr
    {
        private readonly Lazy<IReadOnlyList<Decl>> _decls;
        private readonly Lazy<CXXRecordDecl> _namingClass;
        private readonly Lazy<IReadOnlyList<TemplateArgumentLoc>> _templateArgs;

        private protected OverloadExpr(CXCursor handle, CXCursorKind expectedCursorKind, CX_StmtClass expectedStmtClass) : base(handle, expectedCursorKind, expectedStmtClass)
        {
            if (handle.StmtClass is > CX_StmtClass.CX_StmtClass_LastOverloadExpr or < CX_StmtClass.CX_StmtClass_FirstOverloadExpr)
            {
                throw new ArgumentOutOfRangeException(nameof(handle));
            }

            _decls = new Lazy<IReadOnlyList<Decl>>(() => {
                var numDecls = Handle.NumDecls;
                var decls = new List<Decl>(numDecls);

                for (var i = 0; i < numDecls; i++)
                {
                    var decl = TranslationUnit.GetOrCreate<Decl>(Handle.GetDecl(unchecked((uint)i)));
                    decls.Add(decl);
                }

                return decls;
            });

            _namingClass = new Lazy<CXXRecordDecl>(() => TranslationUnit.GetOrCreate<CXXRecordDecl>(Handle.Referenced));

            _templateArgs = new Lazy<IReadOnlyList<TemplateArgumentLoc>>(() => {
                var templateArgCount = Handle.NumTemplateArguments;
                var templateArgs = new List<TemplateArgumentLoc>(templateArgCount);

                for (var i = 0; i < templateArgCount; i++)
                {
                    var templateArg = TranslationUnit.GetOrCreate(Handle.GetTemplateArgumentLoc(unchecked((uint)i)));
                    templateArgs.Add(templateArg);
                }

                return templateArgs;
            });
        }

        public IReadOnlyList<Decl> Decls => _decls.Value;

        public bool HasExplicitTemplateArgs => Handle.HasExplicitTemplateArgs;

        public bool HasTemplateKeyword => Handle.HasTemplateKeyword;

        public CXXRecordDecl NamingClass => _namingClass.Value;

        public string Name => Handle.Name.CString;

        public uint NumDecls => unchecked((uint)Handle.NumDecls);

        public uint NumTemplateArgs => unchecked((uint)Handle.NumTemplateArguments);

        public IReadOnlyList<TemplateArgumentLoc> TemplateArgs => _templateArgs.Value;
    }
}
