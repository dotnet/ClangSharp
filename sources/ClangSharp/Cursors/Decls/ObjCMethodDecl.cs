// Copyright (c) .NET Foundation and Contributors. All rights reserved. Licensed under the University of Illinois/NCSA Open Source License. See LICENSE.txt in the project root for license information.

using System;
using System.Collections.Generic;
using ClangSharp.Interop;

namespace ClangSharp
{
    public sealed class ObjCMethodDecl : NamedDecl, IDeclContext
    {
        private readonly Lazy<ObjCInterfaceDecl> _classInterface;
        private readonly Lazy<ImplicitParamDecl> _cmdDecl;
        private readonly Lazy<IReadOnlyList<ParmVarDecl>> _parameters;
        private readonly Lazy<Type> _returnType;
        private readonly Lazy<ImplicitParamDecl> _selfDecl;
        private readonly Lazy<Type> _sendResultType;

        internal ObjCMethodDecl(CXCursor handle) : base(handle, handle.Kind, CX_DeclKind.CX_DeclKind_ObjCMethod)
        {
            if (handle.Kind is not CXCursorKind.CXCursor_ObjCInstanceMethodDecl and not CXCursorKind.CXCursor_ObjCClassMethodDecl)
            {
                throw new ArgumentOutOfRangeException(nameof(handle));
            }

            _classInterface = new Lazy<ObjCInterfaceDecl>(() => TranslationUnit.GetOrCreate<ObjCInterfaceDecl>(Handle.GetSubDecl(0)));
            _cmdDecl = new Lazy<ImplicitParamDecl>(() => TranslationUnit.GetOrCreate<ImplicitParamDecl>(Handle.GetSubDecl(1)));

            _parameters = new Lazy<IReadOnlyList<ParmVarDecl>>(() => {
                var parameterCount = Handle.NumArguments;
                var parameters = new List<ParmVarDecl>(parameterCount);

                for (var i = 0; i < parameterCount; i++)
                {
                    var parameter = TranslationUnit.GetOrCreate<ParmVarDecl>(Handle.GetArgument(unchecked((uint)i)));
                    parameters.Add(parameter);
                }

                return parameters;
            });

            _selfDecl = new Lazy<ImplicitParamDecl>(() => TranslationUnit.GetOrCreate<ImplicitParamDecl>(Handle.GetSubDecl(2)));
            _returnType = new Lazy<Type>(() => TranslationUnit.GetOrCreate<Type>(Handle.ReturnType));
            _sendResultType = new Lazy<Type>(() => TranslationUnit.GetOrCreate<Type>(Handle.TypeOperand));
        }

        public new ObjCMethodDecl CanonicalDecl => (ObjCMethodDecl)base.CanonicalDecl;

        public ObjCInterfaceDecl ClassInterface => _classInterface.Value;

        public ImplicitParamDecl CmdDecl => _cmdDecl.Value;

        public bool IsClassMethod => CursorKind == CXCursorKind.CXCursor_ObjCClassMethodDecl;

        public bool IsInstanceMethod => CursorKind == CXCursorKind.CXCursor_ObjCInstanceMethodDecl;

        public bool IsThisDeclarationADefinition => Handle.IsThisDeclarationADefinition;

        public CXObjCDeclQualifierKind ObjCDeclQualifier => Handle.ObjCDeclQualifiers;

        public IReadOnlyList<ParmVarDecl> Parameters => _parameters.Value;

        public Type ReturnType => _returnType.Value;

        public ImplicitParamDecl SelfDecl => _selfDecl.Value;

        public Type SendResultType => _sendResultType.Value;
    }
}
