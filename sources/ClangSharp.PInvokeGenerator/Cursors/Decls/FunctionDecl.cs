using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using ClangSharp.Interop;

namespace ClangSharp
{
    internal class FunctionDecl : DeclaratorDecl
    {
        private readonly List<Decl> _declarations = new List<Decl>();
        private readonly ParmVarDecl[] _parameters;
        private readonly Lazy<Cursor> _specializedTemplate;

        private Stmt _body;

        public FunctionDecl(CXCursor handle, Cursor parent) : base(handle, parent)
        {
            _parameters = new ParmVarDecl[Handle.NumArguments];

            for (uint index = 0; index < Handle.NumArguments; index++)
            {
                var parameterHandle = Handle.GetArgument(index);
                var parmVarDecl = (ParmVarDecl)GetOrAddDecl(parameterHandle);

                _parameters[index] = parmVarDecl;
                parmVarDecl.Visit(clientData: default);
            }

            ReturnType = TranslationUnit.GetOrCreateType(Handle.ResultType, () => Type.Create(Handle.ResultType, TranslationUnit));

            _specializedTemplate = new Lazy<Cursor>(() => {
                var cursor = TranslationUnit.GetOrCreateCursor(Handle.SpecializedCursorTemplate, () => Create(Handle.SpecializedCursorTemplate, this));
                cursor?.Visit(clientData: default);
                return cursor;
            });
        }

        public Stmt Body => _body;

        public IReadOnlyList<Decl> Declarations => _declarations;

        public string DisplayName => Handle.DisplayName.ToString();

        public FunctionType FunctionType => (FunctionType)Type;

        public bool HasDllExport => HasAttrs && Attributes.Any((attr) => attr is DLLExport);

        public bool HasDllImport => HasAttrs && Attributes.Any((attr) => attr is DLLImport);

        public bool IsInlined => Handle.IsFunctionInlined;

        public bool IsVariadic => Handle.IsVariadic;

        public string Mangling => Handle.Mangling.ToString();

        public int NumTemplateArguments => Handle.NumTemplateArguments;

        public IReadOnlyList<ParmVarDecl> Parameters => _parameters;

        public Type ReturnType { get; }

        public Cursor SpecializedTemplate => _specializedTemplate.Value;

        public CX_StorageClass StorageClass => Handle.StorageClass;

        public CXSourceRange GetSpellingNameRange(uint pieceIndex, uint options) => Handle.GetSpellingNameRange(pieceIndex, options);

        public CXTemplateArgumentKind GetTemplateArgumentKind(uint i) => Handle.GetTemplateArgumentKind(i);

        public ulong GetTemplateArgumentUnsignedValue(uint i) => Handle.GetTemplateArgumentUnsignedValue(i);

        public long GetTemplateArgumentValue(uint i) => Handle.GetTemplateArgumentValue(i);

        protected override Decl GetOrAddDecl(CXCursor childHandle)
        {
            var decl = base.GetOrAddDecl(childHandle);
            _declarations.Add(decl);
            return decl;
        }

        protected override Expr GetOrAddExpr(CXCursor childHandle)
        {
            var expr = base.GetOrAddExpr(childHandle);

            Debug.Assert(_body is null);
            _body = expr;

            return expr;
        }

        protected override Stmt GetOrAddStmt(CXCursor childHandle)
        {
            var stmt = base.GetOrAddStmt(childHandle);

            Debug.Assert(_body is null);
            _body = stmt;

            return stmt;
        }
    }
}
