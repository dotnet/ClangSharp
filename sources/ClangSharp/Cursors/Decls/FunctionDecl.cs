// Copyright (c) Microsoft and Contributors. All rights reserved. Licensed under the University of Illinois/NCSA Open Source License. See LICENSE.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Linq;
using ClangSharp.Interop;

namespace ClangSharp
{
    public class FunctionDecl : DeclaratorDecl, IDeclContext, IRedeclarable<FunctionDecl>
    {
        private readonly Lazy<Stmt> _body;
        private readonly Lazy<IReadOnlyList<Decl>> _decls;
        private readonly Lazy<IReadOnlyList<ParmVarDecl>> _parameters;
        private readonly Lazy<Type> _returnType;

        internal FunctionDecl(CXCursor handle) : this(handle, CXCursorKind.CXCursor_FunctionDecl, CX_DeclKind.CX_DeclKind_Function)
        {
        }

        private protected FunctionDecl(CXCursor handle, CXCursorKind expectedCursorKind, CX_DeclKind expectedDeclKind) : base(handle, expectedCursorKind, expectedDeclKind)
        {
            if ((CX_DeclKind.CX_DeclKind_LastFunction < handle.DeclKind) || (handle.DeclKind < CX_DeclKind.CX_DeclKind_FirstFunction))
            {
                throw new ArgumentException(nameof(handle));
            }

            _body = new Lazy<Stmt>(() => CursorChildren.OfType<Stmt>().SingleOrDefault());
            _decls = new Lazy<IReadOnlyList<Decl>>(() => CursorChildren.OfType<Decl>().ToList());
            _parameters = new Lazy<IReadOnlyList<ParmVarDecl>>(() => Decls.OfType<ParmVarDecl>().ToList());
            _returnType = new Lazy<Type>(() => TranslationUnit.GetOrCreate<Type>(Handle.ResultType));
        }

        public Stmt Body => _body.Value;

        public IReadOnlyList<Decl> Decls => _decls.Value;

        public bool IsInlined => Handle.IsFunctionInlined;

        public bool IsVariadic => Handle.IsVariadic;

        public IDeclContext LexicalParent => LexicalDeclContext;

        public IReadOnlyList<ParmVarDecl> Parameters => _parameters.Value;

        public IDeclContext Parent => DeclContext;

        public Type ReturnType => _returnType.Value;

        public CX_StorageClass StorageClass => Handle.StorageClass;
    }
}
