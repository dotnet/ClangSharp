// Copyright (c) .NET Foundation and Contributors. All Rights Reserved. Licensed under the MIT License (MIT). See License.md in the repository root for more information.

using System;
using System.Collections.Generic;
using ClangSharp.Interop;

namespace ClangSharp
{
    public sealed class FriendTemplateDecl : Decl
    {
        private readonly Lazy<NamedDecl> _friendDecl;
        private readonly Lazy<Type> _friendType;
        private readonly Lazy<IReadOnlyList<IReadOnlyList<NamedDecl>>> _templateParameterLists;

        internal FriendTemplateDecl(CXCursor handle) : base(handle, CXCursorKind.CXCursor_UnexposedDecl, CX_DeclKind.CX_DeclKind_FriendTemplate)
        {
            _friendDecl = new Lazy<NamedDecl>(() => TranslationUnit.GetOrCreate<NamedDecl>(Handle.FriendDecl));
            _friendType = new Lazy<Type>(() => TranslationUnit.GetOrCreate<Type>(Handle.TypeOperand));

            _templateParameterLists = new Lazy<IReadOnlyList<IReadOnlyList<NamedDecl>>>(() => {
                var numTemplateParameterLists = Handle.NumTemplateParameterLists;
                var templateParameterLists = new List<IReadOnlyList<NamedDecl>>(numTemplateParameterLists);

                for (var listIndex = 0; listIndex < numTemplateParameterLists; listIndex++)
                {
                    var numTemplateParameters = Handle.GetNumTemplateParameters(unchecked((uint)listIndex));
                    var templateParameterList = new List<NamedDecl>(numTemplateParameters);

                    for (var parameterIndex = 0; parameterIndex < numTemplateParameters; parameterIndex++)
                    {
                        var templateParameter = TranslationUnit.GetOrCreate<NamedDecl>(Handle.GetTemplateParameter(unchecked((uint)listIndex), unchecked((uint)parameterIndex)));
                        templateParameterList.Add(templateParameter);
                    }

                    templateParameterLists.Add(templateParameterList);
                }

                return templateParameterLists;
            });
        }

        public NamedDecl FriendDecl => _friendDecl.Value;

        public Type FriendType => _friendType.Value;

        public uint NumTemplateParameterLists => unchecked((uint)Handle.NumTemplateParameterLists);

        public IReadOnlyList<IReadOnlyList<NamedDecl>> TemplateParameterLists => _templateParameterLists.Value;
    }
}
