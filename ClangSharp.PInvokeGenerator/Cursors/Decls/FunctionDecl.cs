using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace ClangSharp
{
    internal class FunctionDecl : DeclaratorDecl
    {
        private readonly ParmVarDecl[] _parameters;
        private readonly Lazy<Cursor> _specializedTemplate;

        private bool _isDllExport;
        private bool _isDllImport;

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

            _specializedTemplate = new Lazy<Cursor>(() => {
                var cursor = TranslationUnit.GetOrCreateCursor(handle.SpecializedCursorTemplate, () => Create(handle.SpecializedCursorTemplate, this));
                cursor.Visit(clientData: default);
                return cursor;
            });
        }

        public string DisplayName => Handle.DisplayName.ToString();

        public bool HasDllExport => _isDllExport;

        public bool HasDllImport => _isDllImport;

        public bool IsInlined => Handle.IsFunctionInlined;

        public bool IsVariadic => Handle.IsVariadic;

        public string Mangling => Handle.Mangling.ToString();

        public int NumTemplateArguments => Handle.NumTemplateArguments;

        public IReadOnlyList<ParmVarDecl> Parameters => _parameters;

        public Cursor SpecializedTemplate => _specializedTemplate.Value;

        public CX_StorageClass StorageClass => Handle.StorageClass;

        public CXSourceRange GetSpellingNameRange(uint pieceIndex, uint options) => Handle.GetSpellingNameRange(pieceIndex, options);

        public CXTemplateArgumentKind GetTemplateArgumentKind(uint i) => Handle.GetTemplateArgumentKind(i);

        public ulong GetTemplateArgumentUnsignedValue(uint i) => Handle.GetTemplateArgumentUnsignedValue(i);

        public long GetTemplateArgumentValue(uint i) => Handle.GetTemplateArgumentValue(i);

        protected override CXChildVisitResult VisitChildren(CXCursor childHandle, CXCursor handle, CXClientData clientData)
        {
            ValidateVisit(ref handle);

            if (childHandle.IsAttribute)
            {
                switch (childHandle.Kind)
                {
                    case CXCursorKind.CXCursor_DLLExport:
                    {
                        _isDllExport = true;
                        break;
                    }

                    case CXCursorKind.CXCursor_DLLImport:
                    {
                        _isDllImport = true;
                        break;
                    }
                }
            }

            return base.VisitChildren(childHandle, handle, clientData);
        }
    }
}
