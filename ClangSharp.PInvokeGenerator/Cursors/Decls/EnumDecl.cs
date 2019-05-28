﻿using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace ClangSharp
{
    internal sealed class EnumDecl : TagDecl
    {
        private readonly List<EnumConstantDecl> _enumerators = new List<EnumConstantDecl>();
        private readonly Lazy<Type> _integerType;

        public EnumDecl(CXCursor handle, Cursor parent) : base(handle, parent)
        {
            Debug.Assert(handle.Kind == CXCursorKind.CXCursor_EnumDecl);
            _integerType = new Lazy<Type>(() => TranslationUnit.GetOrCreateType(Handle.EnumDecl_IntegerType, () => Type.Create(Handle.EnumDecl_IntegerType, TranslationUnit)));
        }

        public IReadOnlyList<EnumConstantDecl> Enumerators => _enumerators;

        public Type IntegerType => _integerType.Value;

        public bool IsScoped => Handle.EnumDecl_IsScoped;

        protected override Decl GetOrAddDecl(CXCursor childHandle)
        {
            var decl = base.GetOrAddDecl(childHandle);

            if (decl is EnumConstantDecl enumConstantDecl)
            {
                _enumerators.Add(enumConstantDecl);
            }

            return decl;
        }
    }
}
