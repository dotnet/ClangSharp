using System;
using System.Collections.Generic;
using System.Diagnostics;
using ClangSharp;

namespace ClangSharpPInvokeGenerator
{
    internal sealed class TranslationUnit : Cursor
    {
        private readonly Dictionary<CXCursor, Cursor> _visitedCursors = new Dictionary<CXCursor, Cursor>();
        private readonly Dictionary<CXType, Type> _visitedTypes = new Dictionary<CXType, Type>();

        public TranslationUnit(CXCursor handle) : base(handle, parent: null)
        {
            Debug.Assert(handle.Kind == CXCursorKind.CXCursor_TranslationUnit);
        }

        protected override CXChildVisitResult VisitChildren(CXCursor childHandle, CXCursor handle, CXClientData clientData)
        {
            ValidateVisit(ref handle);

            switch (childHandle.Kind)
            {
                case CXCursorKind.CXCursor_UnexposedDecl:
                {
                    return GetOrAddChild<UnexposedDecl>(childHandle).Visit(clientData);
                }

                case CXCursorKind.CXCursor_StructDecl:
                {
                    return GetOrAddChild<StructDecl>(childHandle).Visit(clientData);
                }

                case CXCursorKind.CXCursor_UnionDecl:
                {
                    return GetOrAddChild<UnionDecl>(childHandle).Visit(clientData);
                }

                case CXCursorKind.CXCursor_ClassDecl:
                {
                    return GetOrAddChild<ClassDecl>(childHandle).Visit(clientData);
                }

                case CXCursorKind.CXCursor_EnumDecl:
                {
                    return GetOrAddChild<EnumDecl>(childHandle).Visit(clientData);
                }

                case CXCursorKind.CXCursor_FunctionDecl:
                {
                    return GetOrAddChild<FunctionDecl>(childHandle).Visit(clientData);
                }

                case CXCursorKind.CXCursor_VarDecl:
                {
                    return GetOrAddChild<VarDecl>(childHandle).Visit(clientData);
                }

                case CXCursorKind.CXCursor_TypedefDecl:
                {
                    return GetOrAddChild<TypedefDecl>(childHandle).Visit(clientData);
                }

                case CXCursorKind.CXCursor_CXXMethod:
                {
                    return GetOrAddChild<CXXMethod>(childHandle).Visit(clientData);
                }

                case CXCursorKind.CXCursor_Namespace:
                {
                    return GetOrAddChild<Namespace>(childHandle).Visit(clientData);
                }

                case CXCursorKind.CXCursor_Constructor:
                {
                    return GetOrAddChild<Constructor>(childHandle).Visit(clientData);
                }

                case CXCursorKind.CXCursor_Destructor:
                {
                    return GetOrAddChild<Destructor>(childHandle).Visit(clientData);
                }

                case CXCursorKind.CXCursor_ConversionFunction:
                {
                    return GetOrAddChild<ConversionFunction>(childHandle).Visit(clientData);
                }

                case CXCursorKind.CXCursor_FunctionTemplate:
                {
                    return GetOrAddChild<FunctionTemplate>(childHandle).Visit(clientData);
                }

                case CXCursorKind.CXCursor_ClassTemplate:
                {
                    return GetOrAddChild<ClassTemplate>(childHandle).Visit(clientData);
                }

                case CXCursorKind.CXCursor_UsingDeclaration:
                {
                    return GetOrAddChild<UsingDeclaration>(childHandle).Visit(clientData);
                }

                case CXCursorKind.CXCursor_TypeAliasDecl:
                {
                    return GetOrAddChild<TypeAliasDecl>(childHandle).Visit(clientData);
                }

                default:
                {
                    return base.VisitChildren(childHandle, handle, clientData);
                }
            }
        }

        internal void AddVisitedCursor(Cursor cursor)
        {
            if (_visitedCursors.ContainsKey(cursor.Handle))
            {
                Debugger.Break();
            }
            _visitedCursors.Add(cursor.Handle, cursor);
        }

        internal void AddVisitedType(Type type)
        {
            if (_visitedTypes.ContainsKey(type.Handle))
            {
                Debugger.Break();
            }
            _visitedTypes.Add(type.Handle, type);
        }

        internal Cursor GetOrCreateCursor(CXCursor childHandle, Func<Cursor> createCursor)
        {
            if (childHandle.IsNull || (childHandle.Kind == CXCursorKind.CXCursor_NoDeclFound))
            {
                return null;
            }

            if (!_visitedCursors.TryGetValue(childHandle, out var childCursor))
            {
                childCursor = createCursor();
                Debug.Assert(_visitedCursors.ContainsKey(childHandle));
            }

            return childCursor;
        }

        internal Type GetOrCreateType(CXType handle, Func<Type> createType)
        {
            if (handle.kind == CXTypeKind.CXType_Invalid)
            {
                return null;
            }

            if (!_visitedTypes.TryGetValue(handle, out var type))
            {
                type = createType();
                Debug.Assert(_visitedTypes.ContainsKey(handle));
            }

            return type;
        }
    }
}
