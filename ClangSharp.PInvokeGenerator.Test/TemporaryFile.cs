using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using IOPath = System.IO.Path;

namespace ClangSharp.Test
{
    public sealed class TemporaryFile : IDisposable
    {
        private string _path;

        public TemporaryFile()
        {
            _path = IOPath.GetTempFileName();
        }

        public string Path => _path;

        public void Dispose()
        {
            if (_path != null)
            {
                File.Delete(_path);
                _path = null;
            }
        }

        public Task<string[]> ReadAllLines(CancellationToken cancellationToken = default)
        {
            return File.ReadAllLinesAsync(_path, cancellationToken);
        }

        public Task<string> ReadAllText(CancellationToken cancellationToken = default)
        {
            return File.ReadAllTextAsync(_path, cancellationToken);
        }

        public Task WriteAllLines(IEnumerable<string> contents, CancellationToken cancellationToken = default)
        {
            return File.WriteAllLinesAsync(_path, contents, cancellationToken);
        }

        public Task WriteAllText(string contents, CancellationToken cancellationToken = default)
        {
            return File.WriteAllTextAsync(_path, contents, cancellationToken);
        }
    }
}
