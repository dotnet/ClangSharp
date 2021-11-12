// Copyright (c) .NET Foundation and Contributors. All Rights Reserved. Licensed under the MIT License (MIT). See License.md in the repository root for more information.

using System;
using System.Collections.Generic;
using ClangSharp.Interop;

namespace ClangSharp
{
    public sealed class SizeOfPackExpr : Expr
    {
        private readonly Lazy<NamedDecl> _pack;
        private readonly Lazy<IReadOnlyList<TemplateArgument>> _partialArguments;

        internal SizeOfPackExpr(CXCursor handle) : base(handle, CXCursorKind.CXCursor_SizeOfPackExpr, CX_StmtClass.CX_StmtClass_SizeOfPackExpr)
        {
            _pack = new Lazy<NamedDecl>(() => TranslationUnit.GetOrCreate<NamedDecl>(Handle.Referenced));
            _partialArguments = new Lazy<IReadOnlyList<TemplateArgument>>(() => {
                var templateArgCount = Handle.NumTemplateArguments;
                var templateArgs = new List<TemplateArgument>(templateArgCount);

                for (var i = 0; i < templateArgCount; i++)
                {
                    var templateArg = TranslationUnit.GetOrCreate(Handle.GetTemplateArgument(unchecked((uint)i)));
                    templateArgs.Add(templateArg);
                }

                return templateArgs;
            });
        }

        public NamedDecl Pack => _pack.Value;

        public uint PackLength => unchecked((uint)Handle.PackLength);

        public bool IsPartiallySubstituted => Handle.IsPartiallySubstituted;

        public IReadOnlyList<TemplateArgument> PartialArguments => _partialArguments.Value;
    }
}
