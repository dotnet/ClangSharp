// Copyright (c) Microsoft and Contributors. All rights reserved. Licensed under the University of Illinois/NCSA Open Source License. See LICENSE.txt in the project root for license information.

namespace ClangSharp.Interop
{
    public partial struct CXPlatformAvailability
    {
        public CXString Platform;

        public CXVersion Introduced;

        public CXVersion Deprecated;

        public CXVersion Obsoleted;

        public int Unavailable;

        public CXString Message;
    }
}
