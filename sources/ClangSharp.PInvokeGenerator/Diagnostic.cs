// Copyright (c) .NET Foundation and Contributors. All Rights Reserved. Licensed under the MIT License (MIT). See License.md in the repository root for more information.

using System;
using ClangSharp.Interop;

namespace ClangSharp;

public sealed class Diagnostic : IEquatable<Diagnostic>
{
    private readonly DiagnosticLevel _level;
    private readonly string _location;
    private readonly string _message;

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

    public override bool Equals(object? obj) => (obj is Diagnostic other) && Equals(other);

    public bool Equals(Diagnostic? other)
    {
        if (other is null)
        {
            return false;
        }

        return (_level == other.Level)
            && (_location == other.Location)
            && (_message == other.Message);
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(_level, _location, _message);
    }

    public override string ToString() => string.IsNullOrWhiteSpace(_location) ? $"{_level}: {_message}" : $"{_level} ({_location}): {_message}";
}
