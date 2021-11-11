// Copyright (c) .NET Foundation and Contributors. All Rights Reserved. Licensed under the MIT License (MIT). See License.md in the repository root for more information.

// Ported from https://github.com/llvm/llvm-project/tree/llvmorg-13.0.0/clang/tools/libclang
// Original source is Copyright (c) the LLVM Project and Contributors. Licensed under the Apache License v2.0 with LLVM Exceptions. See NOTICE.txt in the project root for license information.

#ifndef LIBCLANGSHARP_CINDEXDIAGNOSTIC_H
#define LIBCLANGSHARP_CINDEXDIAGNOSTIC_H

#pragma warning(push)
#pragma warning(disable : 4146 4244 4267 4291 4624 4996)

#include <clang-c/CXString.h>
#include <clang-c/Index.h>

#include <cassert>
#include <memory>
#include <utility>
#include <vector>

#pragma warning(pop)

namespace clang {
    class CXDiagnosticImpl;

    class CXDiagnosticSetImpl {
        std::vector<std::unique_ptr<CXDiagnosticImpl>> Diagnostics;
        const bool IsExternallyManaged;
    public:
        CXDiagnosticSetImpl(bool isManaged = false)
            : IsExternallyManaged(isManaged) { }

        virtual ~CXDiagnosticSetImpl();

        size_t getNumDiagnostics() const {
            return Diagnostics.size();
        }

        CXDiagnosticImpl* getDiagnostic(unsigned i) const {
            assert(i < getNumDiagnostics());
            return Diagnostics[i].get();
        }

        void appendDiagnostic(std::unique_ptr<CXDiagnosticImpl> D);

        bool empty() const {
            return Diagnostics.empty();
        }

        bool isExternallyManaged() const { return IsExternallyManaged; }
    };

    class CXDiagnosticImpl {
    public:
        enum Kind {
            StoredDiagnosticKind, LoadedDiagnosticKind,
            CustomNoteDiagnosticKind
        };

        virtual ~CXDiagnosticImpl();

        /// Return the severity of the diagnostic.
        virtual CXDiagnosticSeverity getSeverity() const = 0;

        /// Return the location of the diagnostic.
        virtual CXSourceLocation getLocation() const = 0;

        /// Return the spelling of the diagnostic.
        virtual CXString getSpelling() const = 0;

        /// Return the text for the diagnostic option.
        virtual CXString getDiagnosticOption(CXString* Disable) const = 0;

        /// Return the category of the diagnostic.
        virtual unsigned getCategory() const = 0;

        /// Return the category string of the diagnostic.
        virtual CXString getCategoryText() const = 0;

        /// Return the number of source ranges for the diagnostic.
        virtual unsigned getNumRanges() const = 0;

        /// Return the source ranges for the diagnostic.
        virtual CXSourceRange getRange(unsigned Range) const = 0;

        /// Return the number of FixIts.
        virtual unsigned getNumFixIts() const = 0;

        /// Return the FixIt information (source range and inserted text).
        virtual CXString getFixIt(unsigned FixIt, CXSourceRange* ReplacementRange) const = 0;

        Kind getKind() const { return K; }

        CXDiagnosticSetImpl& getChildDiagnostics() {
            return ChildDiags;
        }

    protected:
        CXDiagnosticImpl(Kind k) : K(k) {}
        CXDiagnosticSetImpl ChildDiags;

        void append(std::unique_ptr<CXDiagnosticImpl> D) {
            ChildDiags.appendDiagnostic(std::move(D));
        }

    private:
        Kind K;
    };
}

#endif
