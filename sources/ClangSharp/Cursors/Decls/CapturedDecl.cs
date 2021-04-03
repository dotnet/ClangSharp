// Copyright (c) Microsoft and Contributors. All rights reserved. Licensed under the University of Illinois/NCSA Open Source License. See LICENSE.txt in the project root for license information.

using ClangSharp.Interop;
using System;
using System.Collections.Generic;

namespace ClangSharp
{
    public sealed class CapturedDecl : Decl, IDeclContext
    {
        private readonly Lazy<ImplicitParamDecl> _contextParam;
        private readonly Lazy<IReadOnlyList<ImplicitParamDecl>> _parameters;

        internal CapturedDecl(CXCursor handle) : base(handle, CXCursorKind.CXCursor_UnexposedDecl, CX_DeclKind.CX_DeclKind_Captured)
        {
            _contextParam = new Lazy<ImplicitParamDecl>(() => TranslationUnit.GetOrCreate<ImplicitParamDecl>(Handle.ContextParam));
            _parameters = new Lazy<IReadOnlyList<ImplicitParamDecl>>(() => {
                var parameterCount = Handle.NumArguments;
                var parameters = new List<ImplicitParamDecl>(parameterCount);

                for (var i = 0; i < parameterCount; i++)
                {
                    var parameter = TranslationUnit.GetOrCreate<ImplicitParamDecl>(Handle.GetArgument(unchecked((uint)i)));
                    parameters.Add(parameter);
                }

                return parameters;
            });
        }

        public ImplicitParamDecl ContextParam => _contextParam.Value;

        public uint ContextParamPosition => unchecked((uint)Handle.ContextParamPosition);

        public bool IsNothrow => Handle.IsNothrow;

        public uint NumParams => unchecked((uint)Handle.NumArguments);

        public IReadOnlyList<ImplicitParamDecl> Parameters => _parameters.Value;
    }
}
