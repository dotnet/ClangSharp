using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace ClangSharp
{
    internal class FunctionDecl : DeclaratorDecl
    {
        private readonly List<Decl> _declarations = new List<Decl>();
        private readonly ParmVarDecl[] _parameters;
        private readonly Lazy<Type> _returnType;
        private readonly Lazy<Cursor> _specializedTemplate;

        private Stmt _body;

        public FunctionDecl(CXCursor handle, Cursor parent) : base(handle, parent)
        {
            _parameters = new ParmVarDecl[Handle.NumArguments];

            for (uint index = 0; index < Handle.NumArguments; index++)
            {
                var parameterHandle = Handle.GetArgument(index);
                var parmVarDecl = GetOrAddChild<ParmVarDecl>(parameterHandle);

                _parameters[index] = parmVarDecl;
                parmVarDecl.Visit(clientData: default);
            }

            _returnType = new Lazy<Type>(() => TranslationUnit.GetOrCreateType(Handle.ResultType, () => Type.Create(Handle.ResultType, TranslationUnit)));

            _specializedTemplate = new Lazy<Cursor>(() => {
                var cursor = TranslationUnit.GetOrCreateCursor(handle.SpecializedCursorTemplate, () => Create(handle.SpecializedCursorTemplate, this));
                cursor.Visit(clientData: default);
                return cursor;
            });
        }

        public IReadOnlyList<Decl> Declarations => _declarations;

        public string DisplayName => Handle.DisplayName.ToString();

        public bool HasDllExport => HasAttrs && Attributes.Any((attr) => attr is DLLExport);

        public bool HasDllImport => HasAttrs && Attributes.Any((attr) => attr is DLLImport);

        public bool IsInlined => Handle.IsFunctionInlined;

        public bool IsVariadic => Handle.IsVariadic;

        public string Mangling => Handle.Mangling.ToString();

        public int NumTemplateArguments => Handle.NumTemplateArguments;

        public IReadOnlyList<ParmVarDecl> Parameters => _parameters;

        public Type ReturnType => _returnType.Value;

        public Cursor SpecializedTemplate => _specializedTemplate.Value;

        public CX_StorageClass StorageClass => Handle.StorageClass;

        public CXSourceRange GetSpellingNameRange(uint pieceIndex, uint options) => Handle.GetSpellingNameRange(pieceIndex, options);

        public CXTemplateArgumentKind GetTemplateArgumentKind(uint i) => Handle.GetTemplateArgumentKind(i);

        public ulong GetTemplateArgumentUnsignedValue(uint i) => Handle.GetTemplateArgumentUnsignedValue(i);

        public long GetTemplateArgumentValue(uint i) => Handle.GetTemplateArgumentValue(i);

        protected TDecl GetOrAddDecl<TDecl>(CXCursor childHandle)
            where TDecl : Decl
        {
            var decl = GetOrAddChild<Decl>(childHandle);
            _declarations.Add(decl);
            return (TDecl)decl;
        }

        protected override CXChildVisitResult VisitChildren(CXCursor childHandle, CXCursor handle, CXClientData clientData)
        {
            ValidateVisit(ref handle);

            if (childHandle.IsDeclaration)
            {
                return GetOrAddDecl<Decl>(childHandle).Visit(clientData);
            }
            else if (childHandle.IsStatement || childHandle.IsExpression)
            {
                var stmt = GetOrAddChild<Stmt>(childHandle);

                Debug.Assert(_body is null);
                _body = stmt;

                return stmt.Visit(clientData);
            }

            return base.VisitChildren(childHandle, handle, clientData);
        }
    }
}
