// Copyright (c) .NET Foundation and Contributors. All Rights Reserved. Licensed under the MIT License (MIT). See License.md in the repository root for more information.

// Ported from https://github.com/llvm/llvm-project/tree/llvmorg-17.0.4/clang/tools/libclang
// Original source is Copyright (c) the LLVM Project and Contributors. Licensed under the Apache License v2.0 with LLVM Exceptions. See NOTICE.txt in the project root for license information.

#ifndef LIBCLANGSHARP_CXLOADEDDIAGNOSTIC_H
#define LIBCLANGSHARP_CXLOADEDDIAGNOSTIC_H

#include "CIndexDiagnostic.h"

#pragma warning(push)
#pragma warning(disable : 4146 4244 4267 4291 4624 4996)

#include <llvm/ADT/StringRef.h>

#pragma warning(pop)

namespace clang {
    class CXLoadedDiagnostic : public CXDiagnosticImpl {
    public:
        CXLoadedDiagnostic()
            : CXDiagnosticImpl(LoadedDiagnosticKind)
            , severity(0)
            , category(0) {
        }

        ~CXLoadedDiagnostic() override;

        /// Return the severity of the diagnostic.
        CXDiagnosticSeverity getSeverity() const override;

        /// Return the location of the diagnostic.
        CXSourceLocation getLocation() const override;

        /// Return the spelling of the diagnostic.
        CXString getSpelling() const override;

        /// Return the text for the diagnostic option.
        CXString getDiagnosticOption(CXString* Disable) const override;

        /// Return the category of the diagnostic.
        unsigned getCategory() const override;

        /// Return the category string of the diagnostic.
        CXString getCategoryText() const override;

        /// Return the number of source ranges for the diagnostic.
        unsigned getNumRanges() const override;

        /// Return the source ranges for the diagnostic.
        CXSourceRange getRange(unsigned Range) const override;

        /// Return the number of FixIts.
        unsigned getNumFixIts() const override;

        /// Return the FixIt information (source range and inserted text).
        CXString getFixIt(unsigned FixIt, CXSourceRange* ReplacementRange) const override;

        static bool classof(const CXDiagnosticImpl* D) {
            return D->getKind() == LoadedDiagnosticKind;
        }

        /// Decode the CXSourceLocation into file, line, column, and offset.
        static void decodeLocation(CXSourceLocation location, CXFile* file, unsigned* line, unsigned* column, unsigned* offset);

        struct Location {
            CXFile file;
            unsigned line;
            unsigned column;
            unsigned offset;

            Location()
                : line(0)
                , column(0)
                , offset(0) {
            }
        };

        Location DiagLoc;

        std::vector<CXSourceRange> Ranges;
        std::vector<std::pair<CXSourceRange, const char*>> FixIts;
        const char* Spelling;
        llvm::StringRef DiagOption;
        llvm::StringRef CategoryText;
        unsigned severity;
        unsigned category;
    };
}

#endif
