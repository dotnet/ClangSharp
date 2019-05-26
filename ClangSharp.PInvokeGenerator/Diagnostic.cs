using System;

namespace ClangSharp
{
    public sealed class Diagnostic
    {
        private readonly DiagnosticLevel _level;
        private readonly string _message;
        private readonly CXSourceLocation _location;

        public Diagnostic(DiagnosticLevel level, string message, CXSourceLocation location)
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

        public CXSourceLocation Location => _location;

        public string Message => _message;

        public override string ToString()
        {
            return $"{_level} ({_location.ToString()}): {_message}";
        }
    }
}
