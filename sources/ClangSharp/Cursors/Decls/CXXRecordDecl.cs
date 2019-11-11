// Copyright (c) Microsoft and Contributors. All rights reserved. Licensed under the University of Illinois/NCSA Open Source License. See LICENSE.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Linq;
using ClangSharp.Interop;

namespace ClangSharp
{
    public class CXXRecordDecl : RecordDecl
    {
        private readonly Lazy<IReadOnlyList<CXXBaseSpecifier>> _bases;
        private readonly Lazy<IReadOnlyList<CXXConstructorDecl>> _ctors;
        private readonly Lazy<CXXDestructorDecl> _destructor;
        private readonly Lazy<IReadOnlyList<FriendDecl>> _friends;
        private readonly Lazy<IReadOnlyList<CXXMethodDecl>> _methods;
        private readonly Lazy<IReadOnlyList<CXXBaseSpecifier>> _vbases;

        internal CXXRecordDecl(CXCursor handle, CXCursorKind expectedKind) : base(handle, expectedKind)
        {
            _bases = new Lazy<IReadOnlyList<CXXBaseSpecifier>>(() => CursorChildren.OfType<CXXBaseSpecifier>().ToList());
            _ctors = new Lazy<IReadOnlyList<CXXConstructorDecl>>(() => Methods.OfType<CXXConstructorDecl>().ToList());
            _destructor = new Lazy<CXXDestructorDecl>(() => Methods.OfType<CXXDestructorDecl>().SingleOrDefault());
            _friends = new Lazy<IReadOnlyList<FriendDecl>>(() => Decls.OfType<FriendDecl>().ToList());
            _methods = new Lazy<IReadOnlyList<CXXMethodDecl>>(() => Decls.OfType<CXXMethodDecl>().ToList());
            _vbases = new Lazy<IReadOnlyList<CXXBaseSpecifier>>(() => Bases.Where((@base) => @base.IsVirtual).ToList());
        }

        public bool IsAbstract => Handle.CXXRecord_IsAbstract;

        public IReadOnlyList<CXXBaseSpecifier> Bases => _bases.Value;

        public IReadOnlyList<CXXConstructorDecl> Ctors => _ctors.Value;

        public CXXDestructorDecl Destructor => _destructor.Value;

        public IReadOnlyList<FriendDecl> Friends => _friends.Value;

        public IReadOnlyList<CXXMethodDecl> Methods => _methods.Value;

        public IReadOnlyList<CXXBaseSpecifier> VBases => _vbases.Value;
    }
}
