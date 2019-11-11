// Copyright (c) Microsoft and Contributors. All rights reserved. Licensed under the University of Illinois/NCSA Open Source License. See LICENSE.txt in the project root for license information.

using System;
using System.Diagnostics;
using ClangSharp.Interop;

namespace ClangSharp
{
    public unsafe class Type : IEquatable<Type>
    {
        private readonly Lazy<Type> _canonicalType;
        private readonly Lazy<TranslationUnit> _translationUnit;

        protected Type(CXType handle, CXTypeKind expectedKind)
        {
            if (handle.kind != expectedKind)
            {
                throw new ArgumentException(nameof(handle));
            }
            Handle = handle;

            _canonicalType = new Lazy<Type>(() => TranslationUnit.GetOrCreate<Type>(Handle.CanonicalType));
            _translationUnit = new Lazy<TranslationUnit>(() => TranslationUnit.GetOrCreate((CXTranslationUnit)Handle.data[1]));
        }

        public string AsString => Handle.Spelling.ToString();

        public Type CanonicalType => _canonicalType.Value;

        public CXType Handle { get; }

        public bool IsLocalConstQualified => Handle.IsConstQualified;

        public CXTypeKind Kind => Handle.kind;

        public string KindSpelling => Handle.KindSpelling.ToString();

        public TranslationUnit TranslationUnit => _translationUnit.Value;

        public static bool operator ==(Type left, Type right) => (left is object) ? ((right is object) && (left.Handle == right.Handle)) : (right is null);

        public static bool operator !=(Type left, Type right) => (left is object) ? ((right is null) || (left.Handle != right.Handle)) : (right is object);

        internal static Type Create(CXType handle)
        {
            Type result;

            switch (handle.kind)
            {
                case CXTypeKind.CXType_Unexposed:
                {
                    result = new Type(handle, CXTypeKind.CXType_Unexposed);
                    break;
                }

                case CXTypeKind.CXType_Void:
                case CXTypeKind.CXType_Bool:
                case CXTypeKind.CXType_Char_U:
                case CXTypeKind.CXType_UChar:
                case CXTypeKind.CXType_UShort:
                case CXTypeKind.CXType_UInt:
                case CXTypeKind.CXType_ULong:
                case CXTypeKind.CXType_ULongLong:
                case CXTypeKind.CXType_Char_S:
                case CXTypeKind.CXType_SChar:
                case CXTypeKind.CXType_WChar:
                case CXTypeKind.CXType_Short:
                case CXTypeKind.CXType_Int:
                case CXTypeKind.CXType_Long:
                case CXTypeKind.CXType_LongLong:
                case CXTypeKind.CXType_Float:
                case CXTypeKind.CXType_Double:
                case CXTypeKind.CXType_LongDouble:
                case CXTypeKind.CXType_NullPtr:
                case CXTypeKind.CXType_Dependent:
                {
                    result = new BuiltinType(handle, handle.kind);
                    break;
                }

                case CXTypeKind.CXType_Pointer:
                {
                    result = new PointerType(handle);
                    break;
                }

                case CXTypeKind.CXType_LValueReference:
                {
                    result = new LValueReferenceType(handle);
                    break;
                }

                case CXTypeKind.CXType_Record:
                {
                    result = new RecordType(handle);
                    break;
                }

                case CXTypeKind.CXType_Enum:
                {
                    result = new EnumType(handle);
                    break;
                }

                case CXTypeKind.CXType_Typedef:
                {
                    result = new TypedefType(handle);
                    break;
                }

                case CXTypeKind.CXType_FunctionProto:
                {
                    result = new FunctionProtoType(handle);
                    break;
                }

                case CXTypeKind.CXType_ConstantArray:
                {
                    result = new ConstantArrayType(handle);
                    break;
                }

                case CXTypeKind.CXType_IncompleteArray:
                {
                    result = new IncompleteArrayType(handle);
                    break;
                }

                case CXTypeKind.CXType_DependentSizedArray:
                {
                    result = new DependentSizedArrayType(handle);
                    break;
                }

                case CXTypeKind.CXType_Elaborated:
                {
                    result = new ElaboratedType(handle);
                    break;
                }

                case CXTypeKind.CXType_Attributed:
                {
                    result = new AttributedType(handle);
                    break;
                }

                default:
                {
                    Debug.WriteLine($"Unhandled type kind: {handle.KindSpelling}.");
                    result = new Type(handle, handle.kind);
                    break;
                }
            }

            return result;
        }

        public override bool Equals(object obj) => (obj is Type other) && Equals(other);

        public bool Equals(Type other) => this == other;

        public override int GetHashCode() => Handle.GetHashCode();

        public override string ToString() => Handle.ToString();
    }
}
