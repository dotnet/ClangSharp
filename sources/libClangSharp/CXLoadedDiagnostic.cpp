// Copyright (c) .NET Foundation and Contributors. All Rights Reserved. Licensed under the MIT License (MIT). See License.md in the repository root for more information.

// Ported from https://github.com/llvm/llvm-project/tree/llvmorg-17.0.4/clang/tools/libclang
// Original source is Copyright (c) the LLVM Project and Contributors. Licensed under the Apache License v2.0 with LLVM Exceptions. See NOTICE.txt in the project root for license information.

#include "CXLoadedDiagnostic.h"
#include "CXString.h"

#pragma warning(push)
#pragma warning(disable : 4146 4244 4267 4291 4624 4996)

#include <clang/Frontend/SerializedDiagnostics.h>
#include <llvm/ADT/Twine.h>

#pragma warning(pop)

namespace clang {
    CXLoadedDiagnostic::~CXLoadedDiagnostic() {
    }

    CXDiagnosticSeverity CXLoadedDiagnostic::getSeverity() const {
        // FIXME: Fail more softly if the diagnostic level is unknown?
        auto severityAsLevel = static_cast<serialized_diags::Level>(severity);
        assert(severity == static_cast<unsigned>(severityAsLevel) && "unknown serialized diagnostic level");

        switch (severityAsLevel) {
#define CASE(X) case serialized_diags::X: return CXDiagnostic_##X;
            CASE(Ignored)
            CASE(Note)
            CASE(Warning)
            CASE(Error)
            CASE(Fatal)
#undef CASE
            // The 'Remark' level isn't represented in the stable API.
            case serialized_diags::Remark: return CXDiagnostic_Warning;
        }

        llvm_unreachable("Invalid diagnostic level");
    }

    static CXSourceLocation makeLocation(const CXLoadedDiagnostic::Location* DLoc) {
        // The lowest bit of ptr_data[0] is always set to 1 to indicate this
        // is a persistent diagnostic.
        uintptr_t V = (uintptr_t)DLoc;
        V |= 0x1;
        CXSourceLocation Loc = {
            {
                (void*)V,
                nullptr
            },
            0
        };
        return Loc;
    }

    CXSourceLocation CXLoadedDiagnostic::getLocation() const {
        // The lowest bit of ptr_data[0] is always set to 1 to indicate this
        // is a persistent diagnostic.
        return makeLocation(&DiagLoc);
    }

    CXString CXLoadedDiagnostic::getSpelling() const {
        return cxstring::createRef(Spelling);
    }

    CXString CXLoadedDiagnostic::getDiagnosticOption(CXString* Disable) const {
        if (DiagOption.empty()) {
            return cxstring::createEmpty();
        }

        // FIXME: possibly refactor with logic in CXStoredDiagnostic.
        if (Disable) {
            *Disable = cxstring::createDup((llvm::Twine("-Wno-") + DiagOption).str());
        }

        return cxstring::createDup((llvm::Twine("-W") + DiagOption).str());
    }

    unsigned CXLoadedDiagnostic::getCategory() const {
        return category;
    }

    CXString CXLoadedDiagnostic::getCategoryText() const {
        return cxstring::createDup(CategoryText);
    }

    unsigned CXLoadedDiagnostic::getNumRanges() const {
        return Ranges.size();
    }

    CXSourceRange CXLoadedDiagnostic::getRange(unsigned Range) const {
        assert(Range < Ranges.size());
        return Ranges[Range];
    }

    unsigned CXLoadedDiagnostic::getNumFixIts() const {
        return FixIts.size();
    }

    CXString CXLoadedDiagnostic::getFixIt(unsigned FixIt, CXSourceRange* ReplacementRange) const {
        assert(FixIt < FixIts.size());

        if (ReplacementRange) {
            *ReplacementRange = FixIts[FixIt].first;
        }
        return cxstring::createRef(FixIts[FixIt].second);
    }

    void CXLoadedDiagnostic::decodeLocation(CXSourceLocation location, CXFile* file, unsigned int* line, unsigned int* column, unsigned int* offset) {
        // CXSourceLocation consists of the following fields:
        //
        //   void* ptr_data[2];
        //   unsigned int_data;
        //
        // The lowest bit of ptr_data[0] is always set to 1 to indicate this
        // is a persistent diagnostic.
        //
        // For now, do the unoptimized approach and store the data in a side
        // data structure.  We can optimize this case later.

        uintptr_t V = (uintptr_t)location.ptr_data[0];
        assert((V & 0x1) == 1);
        V &= ~(uintptr_t)1;

        const Location& Loc = *((Location*)V);

        if (file) {
            *file = Loc.file;
        }

        if (line) {
            *line = Loc.line;
        }

        if (column) {
            *column = Loc.column;
        }

        if (offset) {
            *offset = Loc.offset;
        }
    }
}
