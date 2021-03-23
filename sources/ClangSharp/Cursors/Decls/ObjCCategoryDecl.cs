// Copyright (c) Microsoft and Contributors. All rights reserved. Licensed under the University of Illinois/NCSA Open Source License. See LICENSE.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Linq;
using ClangSharp.Interop;

namespace ClangSharp
{
    public sealed class ObjCCategoryDecl : ObjCContainerDecl
    {
        private readonly Lazy<ObjCInterfaceDecl> _classInterface;
        private readonly Lazy<ObjCCategoryImplDecl> _implementation;
        private readonly Lazy<IReadOnlyList<ObjCIvarDecl>> _ivars;
        private readonly Lazy<ObjCCategoryDecl> _nextClassCategory;
        private readonly Lazy<ObjCCategoryDecl> _nextClassCategoryRaw;
        private readonly Lazy<IReadOnlyList<ObjCProtocolDecl>> _protocols;
        private readonly Lazy<IReadOnlyList<ObjCTypeParamDecl>> _typeParamList;

        internal ObjCCategoryDecl(CXCursor handle) : base(handle, CXCursorKind.CXCursor_ObjCCategoryDecl, CX_DeclKind.CX_DeclKind_ObjCCategory)
        {
            _classInterface = new Lazy<ObjCInterfaceDecl>(() => TranslationUnit.GetOrCreate<ObjCInterfaceDecl>(Handle.GetSubDecl(0)));
            _implementation = new Lazy<ObjCCategoryImplDecl>(() => TranslationUnit.GetOrCreate<ObjCCategoryImplDecl>(Handle.GetSubDecl(1)));
            _ivars = new Lazy<IReadOnlyList<ObjCIvarDecl>>(() => Decls.OfType<ObjCIvarDecl>().ToList());
            _nextClassCategory = new Lazy<ObjCCategoryDecl>(() => TranslationUnit.GetOrCreate<ObjCCategoryDecl>(Handle.GetSubDecl(2)));
            _nextClassCategoryRaw = new Lazy<ObjCCategoryDecl>(() => TranslationUnit.GetOrCreate<ObjCCategoryDecl>(Handle.GetSubDecl(3)));

            _protocols = new Lazy<IReadOnlyList<ObjCProtocolDecl>>(() => {
                var numProtocols = Handle.NumProtocols;
                var protocols = new List<ObjCProtocolDecl>(numProtocols);

                for (int i = 0; i < numProtocols; i++)
                {
                    var protocol = TranslationUnit.GetOrCreate<ObjCProtocolDecl>(Handle.GetProtocol(unchecked((uint)i)));
                    protocols.Add(protocol);
                }

                return protocols;
            });

            _typeParamList = new Lazy<IReadOnlyList<ObjCTypeParamDecl>>(() => {
                var numTypeParams = Handle.NumArguments;
                var typeParams = new List<ObjCTypeParamDecl>(numTypeParams);

                for (int i = 0; i < numTypeParams; i++)
                {
                    var typeParam = TranslationUnit.GetOrCreate<ObjCTypeParamDecl>(Handle.GetArgument(unchecked((uint)i)));
                    typeParams.Add(typeParam);
                }

                return typeParams;
            });
        }

        public ObjCInterfaceDecl ClassInterface => _classInterface.Value;

        public ObjCCategoryImplDecl Implementation => _implementation.Value;

        public bool IsClassExtension => Handle.IsClassExtension;

        public IReadOnlyList<ObjCIvarDecl> Ivars => _ivars.Value;

        public ObjCCategoryDecl NextClassCategory => _nextClassCategory.Value;

        public ObjCCategoryDecl NextClassCategoryRaw => _nextClassCategoryRaw.Value;

        public IReadOnlyList<ObjCProtocolDecl> Protocols => _protocols.Value;

        public IReadOnlyList<ObjCProtocolDecl> ReferencedProtocols => Protocols;

        public IReadOnlyList<ObjCTypeParamDecl> TypeParamList => _typeParamList.Value;
    }
}
