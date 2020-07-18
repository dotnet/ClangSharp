// Copyright (c) Microsoft and Contributors. All rights reserved. Licensed under the University of Illinois/NCSA Open Source License. See LICENSE.txt in the project root for license information.

using System;
using System.Collections.Generic;
using ClangSharp.Interop;

namespace ClangSharp
{
    public sealed class AutoType : DeducedType
    {
        private readonly Lazy<IReadOnlyList<TemplateArgument>> _templateArgs;

        internal AutoType(CXType handle) : base(handle, CXTypeKind.CXType_Auto, CX_TypeClass.CX_TypeClass_Auto)
        {
            _templateArgs = new Lazy<IReadOnlyList<TemplateArgument>>(() => {
                var templateArgCount = Handle.NumTemplateArguments;
                var templateArgs = new List<TemplateArgument>(templateArgCount);

                for (int i = 0; i < templateArgCount; i++)
                {
                    var templateArg = new TemplateArgument(this, unchecked((uint)i));
                    templateArgs.Add(templateArg);
                }

                return templateArgs;
            });
        }

        public IReadOnlyList<TemplateArgument> Args => _templateArgs.Value;
    }
}
