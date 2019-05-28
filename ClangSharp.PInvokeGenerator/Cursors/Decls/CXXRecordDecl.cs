using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace ClangSharp
{
    internal class CXXRecordDecl : RecordDecl
    {
        private readonly List<CXXBaseSpecifier> _bases = new List<CXXBaseSpecifier>();
        private readonly List<CXXConstructorDecl> _constructors = new List<CXXConstructorDecl>();
        private readonly List<FriendDecl> _friends = new List<FriendDecl>();
        private readonly List<CXXMethodDecl> _methods = new List<CXXMethodDecl>();
        private readonly List<CXXBaseSpecifier> _virtualBases = new List<CXXBaseSpecifier>();
        private readonly Lazy<Cursor> _specializedTemplate;

        private CXXDestructorDecl _destructor;

        public CXXRecordDecl(CXCursor handle, Cursor parent) : base(handle, parent)
        {
            _specializedTemplate = new Lazy<Cursor>(() => {
                var cursor = TranslationUnit.GetOrCreateCursor(handle.SpecializedCursorTemplate, () => Create(handle.SpecializedCursorTemplate, this));
                cursor.Visit(clientData: default);
                return cursor;
            });
        }

        public bool IsAbstract => Handle.CXXRecord_IsAbstract;

        public IReadOnlyList<CXXBaseSpecifier> Bases => _bases;

        public IReadOnlyList<CXXConstructorDecl> Constructors => _constructors;

        public CXXDestructorDecl Destructor => _destructor;

        public IReadOnlyList<FriendDecl> Friends => _friends;

        public IReadOnlyList<CXXMethodDecl> Methods => _methods;

        public IReadOnlyList<CXXBaseSpecifier> VirtualBases => _virtualBases;

        public Cursor SpecializedTemplate => _specializedTemplate.Value;

        protected override Decl GetOrAddDecl(CXCursor childHandle)
        {
            var decl = base.GetOrAddDecl(childHandle);

            if (decl is CXXMethodDecl cxxMethodDecl)
            {
                if (decl is CXXConstructorDecl cxxConstructorDecl)
                {
                    _constructors.Add(cxxConstructorDecl);
                }
                else if (decl is CXXDestructorDecl cxxDestructorDecl)
                {
                    Debug.Assert(_destructor is null);
                    _destructor = cxxDestructorDecl;
                }

                _methods.Add(cxxMethodDecl);
            }

            return decl;
        }

        protected override Ref GetOrAddRef(CXCursor childHandle)
        {
            var @ref = base.GetOrAddRef(childHandle);

            if (@ref is CXXBaseSpecifier cxxBaseSpecifier)
            {
                if (cxxBaseSpecifier.IsVirtual)
                {
                    _virtualBases.Add(cxxBaseSpecifier);
                }

                _bases.Add(cxxBaseSpecifier);
            }

            return @ref;
        }
    }
}
