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
            _bases = new Lazy<IReadOnlyList<CXXBaseSpecifier>>(() => CursorChildren.Where((cursor) => cursor is CXXBaseSpecifier).Cast<CXXBaseSpecifier>().ToList());
            _ctors = new Lazy<IReadOnlyList<CXXConstructorDecl>>(() => Methods.Where((method) => method is CXXConstructorDecl).Cast<CXXConstructorDecl>().ToList());
            _destructor = new Lazy<CXXDestructorDecl>(() => Methods.Where((method) => method is CXXDestructorDecl).Cast<CXXDestructorDecl>().Single());
            _friends = new Lazy<IReadOnlyList<FriendDecl>>(() => Decls.Where((decl) => decl is FriendDecl).Cast<FriendDecl>().ToList());
            _methods = new Lazy<IReadOnlyList<CXXMethodDecl>>(() => Decls.Where((decl) => decl is CXXMethodDecl).Cast<CXXMethodDecl>().ToList());
            _vbases = new Lazy<IReadOnlyList<CXXBaseSpecifier>>(() => Bases.Where((@base) => @base.IsVirtual).Cast<CXXBaseSpecifier>().ToList());
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
