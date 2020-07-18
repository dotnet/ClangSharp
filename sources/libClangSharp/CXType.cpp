// Copyright (c) Microsoft and Contributors. All rights reserved. Licensed under the University of Illinois/NCSA Open Source License. See LICENSE.txt in the project root for license information.

// Ported from https://github.com/llvm/llvm-project/tree/llvmorg-10.0.0/clang/tools/libclang
// Original source is Copyright (c) the LLVM Project and Contributors. Licensed under the Apache License v2.0 with LLVM Exceptions. See NOTICE.txt in the project root for license information.

#include "ClangSharp.h"
#include "CXTranslationUnit.h"
#include "CXType.h"

using namespace clang;

QualType GetQualType(CXType CT) {
    return QualType::getFromOpaquePtr(CT.data[0]);
}

CXTranslationUnit GetTypeTU(CXType CT) {
    return static_cast<CXTranslationUnit>(CT.data[1]);
}

namespace clang::cxtype {
    static CXTypeKind GetBuiltinTypeKind(const BuiltinType* BT) {
#define BTCASE(K) case BuiltinType::K: return CXType_##K
        switch (BT->getKind()) {
            BTCASE(Void);
            BTCASE(Bool);
            BTCASE(Char_U);
            BTCASE(UChar);
            BTCASE(Char16);
            BTCASE(Char32);
            BTCASE(UShort);
            BTCASE(UInt);
            BTCASE(ULong);
            BTCASE(ULongLong);
            BTCASE(UInt128);
            BTCASE(Char_S);
            BTCASE(SChar);
        case BuiltinType::WChar_S: return CXType_WChar;
        case BuiltinType::WChar_U: return CXType_WChar;
            BTCASE(Short);
            BTCASE(Int);
            BTCASE(Long);
            BTCASE(LongLong);
            BTCASE(Int128);
            BTCASE(Half);
            BTCASE(Float);
            BTCASE(Double);
            BTCASE(LongDouble);
            BTCASE(ShortAccum);
            BTCASE(Accum);
            BTCASE(LongAccum);
            BTCASE(UShortAccum);
            BTCASE(UAccum);
            BTCASE(ULongAccum);
            BTCASE(Float16);
            BTCASE(Float128);
            BTCASE(NullPtr);
            BTCASE(Overload);
            BTCASE(Dependent);
            BTCASE(ObjCId);
            BTCASE(ObjCClass);
            BTCASE(ObjCSel);
#define IMAGE_TYPE(ImgType, Id, SingletonId, Access, Suffix) BTCASE(Id);
#include "clang/Basic/OpenCLImageTypes.def"
#undef IMAGE_TYPE
#define EXT_OPAQUE_TYPE(ExtType, Id, Ext) BTCASE(Id);
#include "clang/Basic/OpenCLExtensionTypes.def"
            BTCASE(OCLSampler);
            BTCASE(OCLEvent);
            BTCASE(OCLQueue);
            BTCASE(OCLReserveID);
        default:
            return CXType_Unexposed;
        }
#undef BTCASE
    }

    static CXTypeKind GetTypeKind(QualType T) {
        const Type* TP = T.getTypePtrOrNull();
        if (!TP)
            return CXType_Invalid;

#define TKCASE(K) case Type::K: return CXType_##K
        switch (TP->getTypeClass()) {
        case Type::Builtin:
            return GetBuiltinTypeKind(cast<BuiltinType>(TP));
            TKCASE(Complex);
            TKCASE(Pointer);
            TKCASE(BlockPointer);
            TKCASE(LValueReference);
            TKCASE(RValueReference);
            TKCASE(Record);
            TKCASE(Enum);
            TKCASE(Typedef);
            TKCASE(ObjCInterface);
            TKCASE(ObjCObject);
            TKCASE(ObjCObjectPointer);
            TKCASE(ObjCTypeParam);
            TKCASE(FunctionNoProto);
            TKCASE(FunctionProto);
            TKCASE(ConstantArray);
            TKCASE(IncompleteArray);
            TKCASE(VariableArray);
            TKCASE(DependentSizedArray);
            TKCASE(Vector);
            TKCASE(ExtVector);
            TKCASE(MemberPointer);
            TKCASE(Auto);
            TKCASE(Elaborated);
            TKCASE(Pipe);
            TKCASE(Attributed);
        default:
            return CXType_Unexposed;
        }
#undef TKCASE
    }

    CXType MakeCXType(QualType T, CXTranslationUnit TU) {
        CXTypeKind TK = CXType_Invalid;

        if (TU && !T.isNull()) {
            // Handle attributed types as the original type
            if (auto* ATT = T->getAs<AttributedType>()) {
                if (!(TU->ParsingOptions & CXTranslationUnit_IncludeAttributedTypes)) {
                    // Return the equivalent type which represents the canonically
                    // equivalent type.
                    return MakeCXType(ATT->getEquivalentType(), TU);
                }
            }
            // Handle paren types as the original type
            if (auto* PTT = T->getAs<ParenType>()) {
                return MakeCXType(PTT->getInnerType(), TU);
            }

            ASTContext& Ctx = cxtu::getASTUnit(TU)->getASTContext();
            if (Ctx.getLangOpts().ObjC) {
                QualType UnqualT = T.getUnqualifiedType();
                if (Ctx.isObjCIdType(UnqualT))
                    TK = CXType_ObjCId;
                else if (Ctx.isObjCClassType(UnqualT))
                    TK = CXType_ObjCClass;
                else if (Ctx.isObjCSelType(UnqualT))
                    TK = CXType_ObjCSel;
            }

            /* Handle decayed types as the original type */
            if (const DecayedType* DT = T->getAs<DecayedType>()) {
                return MakeCXType(DT->getOriginalType(), TU);
            }
        }
        if (TK == CXType_Invalid)
            TK = GetTypeKind(T);

        CXType CT = { TK, { TK == CXType_Invalid ? nullptr
                                                 : T.getAsOpaquePtr(), TU } };
        return CT;
    }
}
