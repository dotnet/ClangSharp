// Copyright (c) Microsoft and Contributors. All rights reserved. Licensed under the University of Illinois/NCSA Open Source License. See LICENSE.txt in the project root for license information.

using System;
using ClangSharp.Interop;

namespace ClangSharp
{
    public sealed class TemplateArgument
    {
        private readonly Decl _parentDecl;
        private readonly Type _parentType;
        private readonly uint _index;
        private readonly Lazy<ValueDecl> _asDecl;
        private readonly Lazy<Expr> _asExpr;
        private readonly Lazy<Type> _asType;
        private readonly Lazy<Type> _integralType;
        private readonly Lazy<Type> _nullPtrType;

        internal TemplateArgument(Decl parentDecl, uint index)
        {
            _parentDecl = parentDecl;
            _index = index;
            _asDecl = new Lazy<ValueDecl>(() => _parentDecl.TranslationUnit.GetOrCreate<ValueDecl>(_parentDecl.Handle.GetTemplateArgumentAsDecl(_index)));
            _asExpr = new Lazy<Expr>(() => _parentDecl.TranslationUnit.GetOrCreate<Expr>(_parentDecl.Handle.GetTemplateArgumentAsExpr(_index)));
            _asType = new Lazy<Type>(() => _parentDecl.TranslationUnit.GetOrCreate<Type>(_parentDecl.Handle.GetTemplateArgumentAsType(_index)));
            _integralType = new Lazy<Type>(() => _parentDecl.TranslationUnit.GetOrCreate<Type>(_parentDecl.Handle.GetTemplateArgumentIntegralType(_index)));
            _nullPtrType = new Lazy<Type>(() => _parentDecl.TranslationUnit.GetOrCreate<Type>(_parentDecl.Handle.GetTemplateArgumentNullPtrType(_index)));
        }

        internal TemplateArgument(Type parentType, uint index)
        {
            _parentType = parentType;
            _index = index;
            _asDecl = new Lazy<ValueDecl>(() => _parentDecl.TranslationUnit.GetOrCreate<ValueDecl>(_parentType.Handle.GetTemplateArgumentAsDecl(_index)));
            _asExpr = new Lazy<Expr>(() => _parentDecl.TranslationUnit.GetOrCreate<Expr>(_parentType.Handle.GetTemplateArgumentAsExpr(_index)));
            _asType = new Lazy<Type>(() => _parentDecl.TranslationUnit.GetOrCreate<Type>(_parentType.Handle.GetTemplateArgumentAsType(_index)));
            _integralType = new Lazy<Type>(() => _parentDecl.TranslationUnit.GetOrCreate<Type>(_parentType.Handle.GetTemplateArgumentIntegralType(_index)));
            _nullPtrType = new Lazy<Type>(() => _parentDecl.TranslationUnit.GetOrCreate<Type>(_parentType.Handle.GetTemplateArgumentNullPtrType(_index)));
        }

        public ValueDecl AsDecl => _asDecl.Value;

        public Expr AsExpr => _asExpr.Value;

        public long AsIntegral => (_parentDecl != null) ? _parentDecl.Handle.GetTemplateArgumentAsIntegral(_index) : _parentType.Handle.GetTemplateArgumentAsIntegral(_index);

        public Type AsType => _asType.Value;

        public Type IntegralType => _integralType.Value;

        public bool IsNull => Kind == CXTemplateArgumentKind.CXTemplateArgumentKind_Null;

        public CXTemplateArgumentKind Kind => (_parentDecl != null) ? _parentDecl.Handle.GetTemplateArgumentKind(_index) : _parentType.Handle.GetTemplateArgumentKind(_index);

        public Type NullPtrType => _nullPtrType.Value;
    }
}
