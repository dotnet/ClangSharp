// Copyright (c) Microsoft and Contributors. All rights reserved. Licensed under the University of Illinois/NCSA Open Source License. See LICENSE.txt in the project root for license information.

using System;
using System.Collections.Generic;
using ClangSharp.Interop;

namespace ClangSharp
{
    public sealed class FriendDecl : Decl
    {
        private readonly Lazy<NamedDecl> _friendNamedDecl;
        private readonly Lazy<IReadOnlyList<IReadOnlyList<NamedDecl>>> _friendTypeTemplateParameterLists;

        internal FriendDecl(CXCursor handle) : base(handle, CXCursorKind.CXCursor_FriendDecl, CX_DeclKind.CX_DeclKind_Friend)
        {
            _friendNamedDecl = new Lazy<NamedDecl>(() => TranslationUnit.GetOrCreate<NamedDecl>(Handle.FriendDecl));

            _friendTypeTemplateParameterLists = new Lazy<IReadOnlyList<IReadOnlyList<NamedDecl>>>(() => {
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

        public NamedDecl FriendNamedDecl => _friendNamedDecl.Value;

        public bool IsUnsupportedFriend => Handle.IsUnsupportedFriend;

        public uint FriendTypeNumTemplateParameterLists => unchecked((uint)Handle.NumTemplateParameterLists);

        public IReadOnlyList<IReadOnlyList<NamedDecl>> FriendTypeTemplateParameterLists => _friendTypeTemplateParameterLists.Value;
    }
}
