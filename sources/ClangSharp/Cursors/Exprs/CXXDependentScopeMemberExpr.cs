// Copyright (c) Microsoft and Contributors. All rights reserved. Licensed under the University of Illinois/NCSA Open Source License. See LICENSE.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using ClangSharp.Interop;

namespace ClangSharp
{
    public sealed class CXXDependentScopeMemberExpr : Expr
    {
        private readonly Lazy<Type> _baseType;
        private readonly Lazy<NamedDecl> _firstQualifierFoundInScope;
        private readonly Lazy<IReadOnlyList<TemplateArgumentLoc>> _templateArgs;

        internal CXXDependentScopeMemberExpr(CXCursor handle) : base(handle, CXCursorKind.CXCursor_MemberRefExpr, CX_StmtClass.CX_StmtClass_CXXDependentScopeMemberExpr)
        {
            Debug.Assert(NumChildren is 0 or 1);

            _baseType = new Lazy<Type>(() => TranslationUnit.GetOrCreate<Type>(Handle.TypeOperand));
            _firstQualifierFoundInScope = new Lazy<NamedDecl>(() => TranslationUnit.GetOrCreate<NamedDecl>(handle.Referenced));
            _templateArgs = new Lazy<IReadOnlyList<TemplateArgumentLoc>>(() => {
                var templateArgCount = Handle.NumTemplateArguments;
                var templateArgs = new List<TemplateArgumentLoc>(templateArgCount);

                for (int i = 0; i < templateArgCount; i++)
                {
                    var templateArg = TranslationUnit.GetOrCreate(Handle.GetTemplateArgumentLoc(unchecked((uint)i)));
                    templateArgs.Add(templateArg);
                }

                return templateArgs;
            });
        }

        public Expr Base => (Expr)Children.SingleOrDefault();

        public Type BaseType => _baseType.Value;

        public NamedDecl FirstQualifierFoundInScope => _firstQualifierFoundInScope.Value;

        public bool HasExplicitTemplateArgs => Handle.HasExplicitTemplateArgs;

        public bool HasTemplateKeyword => Handle.HasTemplateKeyword;

        public bool IsArrow => Handle.IsArrow;

        public bool IsImplicitAccess => (Base is null) || Base.IsImplicitCXXThis;

        public string MemberName => Handle.Name.CString;

        public uint NumTemplateArgs => unchecked((uint)Handle.NumTemplateArguments);

        public IReadOnlyList<TemplateArgumentLoc> TemplateArgs => _templateArgs.Value;
    }
}
