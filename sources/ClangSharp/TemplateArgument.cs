// Copyright (c) Microsoft and Contributors. All rights reserved. Licensed under the University of Illinois/NCSA Open Source License. See LICENSE.txt in the project root for license information.

using System;
using ClangSharp.Interop;

namespace ClangSharp
{
    public sealed class TemplateArgument
    {
        private readonly ClassTemplateSpecializationDecl _parentDecl;
        private readonly uint _index;
        private readonly Lazy<Type> _asType;

        internal TemplateArgument(ClassTemplateSpecializationDecl parentDecl, uint index)
        {
            _parentDecl = parentDecl;
            _index = index;
            _asType = new Lazy<Type>(() => _parentDecl.TranslationUnit.GetOrCreate<Type>(_parentDecl.Handle.GetTemplateArgumentType(_index)));
        }

        public long AsIntegral => _parentDecl.Handle.GetTemplateArgumentValue(_index);

        public Type AsType => _asType.Value;

        public bool IsNull => Kind == CXTemplateArgumentKind.CXTemplateArgumentKind_Null;

        public CXTemplateArgumentKind Kind => _parentDecl.Handle.GetTemplateArgumentKind(_index);
    }
}
