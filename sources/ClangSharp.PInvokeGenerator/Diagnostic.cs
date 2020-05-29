// Copyright (c) Microsoft and Contributors. All rights reserved. Licensed under the University of Illinois/NCSA Open Source License. See LICENSE.txt in the project root for license information.

using System;
using ClangSharp.Interop;

namespace ClangSharp
{
    public sealed class Diagnostic : IEquatable<Diagnostic>
    {
        private readonly DiagnosticLevel _level;
        private readonly string _message;
        private readonly string _location;

        public Diagnostic(DiagnosticLevel level, string message) : this(level, message, "")
        {
        }

        public Diagnostic(DiagnosticLevel level, string message, CXSourceLocation location) : this(level, message, location.ToString().Replace('\\', '/'))
        {
        }

        public Diagnostic(DiagnosticLevel level, string message, string location)
        {
            if (string.IsNullOrWhiteSpace(message))
            {
                throw new ArgumentNullException(nameof(message));
            }

            _level = level;
            _message = message;
            _location = location;
        }

        public DiagnosticLevel Level => _level;

        public string Location => _location;

        public string Message => _message;

        public override bool Equals(object obj)
        {
            return (obj is Diagnostic other) && Equals(other);
        }

        public bool Equals(Diagnostic other)
        {
            return (_level == other.Level)
                && (_location == other.Location)
                && (_message == other.Message);
        }

        public override string ToString()
        {
            if (string.IsNullOrWhiteSpace(_location))
            {
                return $"{_level}: {_message}";
            }
            return $"{_level} ({_location}): {_message}";
        }
    }
}
