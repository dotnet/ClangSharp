// Copyright (c) .NET Foundation and Contributors. All Rights Reserved. Licensed under the MIT License (MIT). See License.md in the repository root for more information.

using System;
using System.Runtime.InteropServices;
using ClangSharp.Interop;
using NUnit.Framework;
using static ClangSharp.Interop.CXErrorCode;

namespace ClangSharp.UnitTests;

// NOTE: To make those tests work on both Windows and *nix,
//       begin any absolute path with "$ROOT$", which the TestVFO infra will replace
//       with either "C:" (Windows) or nothing (*nix) so that clang understands the path.
//       (Technically, /a/path/like/this is a path relative to the root of the current drive
//        on Windows, but clang doesn't seem to support this)
public class CXVirtualFileOverlayTest
{
    internal sealed class TestVFO(string? contents) : IDisposable
    {
        public CXVirtualFileOverlay VFO = CXVirtualFileOverlay.Create(options: 0);

        private readonly string? _contents = Fix(contents);
        private bool _isDisposed;

        ~TestVFO()
        {
            Dispose(isDisposing: false);
        }

        public void Map(string? vPath, string? rPath)
        {
            vPath = Fix(vPath);
            rPath = Fix(rPath);

            var err = VFO.AddFileMapping(vPath, rPath);
            Assert.That(err, Is.EqualTo(CXError_Success));
        }

        public void MapError(string? vPath, string? rPath, CXErrorCode expErr)
        {
            vPath = Fix(vPath);
            rPath = Fix(rPath);

            var err = VFO.AddFileMapping(vPath, rPath);
            Assert.That(err, Is.EqualTo(expErr));
        }

        private static string? Fix(string? text)
        {
            if (text is null)
            {
                return null;
            }

            var replacement = RuntimeInformation.IsOSPlatform(OSPlatform.Windows) ? "C:" : "";
            return text.Replace("$ROOT$", replacement, StringComparison.Ordinal);
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        private void Dispose(bool isDisposing)
        {
            if (_isDisposed)
            {
                return;
            }
            _isDisposed = true;

            if (_contents != null)
            {
                var buffer = VFO.WriteToBuffer(options: 0, errorCode: out _);
                Assert.That(buffer.AsString(), Is.EqualTo(_contents));
                buffer.ClangFree();
            }

            VFO.Dispose();
        }
    }

    [Test]
    public void Basic()
    {
        var contents =
            "{\n"
            + "  'version': 0,\n"
            + "  'roots': [\n"
            + "    {\n"
            + "      'type': 'directory',\n"
            + "      'name': \"$ROOT$/path/virtual\",\n"
            + "      'contents': [\n"
            + "        {\n"
            + "          'type': 'file',\n"
            + "          'name': \"foo.h\",\n"
            + "          'external-contents': \"$ROOT$/real/foo.h\"\n"
            + "        }\n"
            + "      ]\n"
            + "    }\n"
            + "  ]\n"
            + "}\n";

        using var t = new TestVFO(contents);
        t.Map(@"$ROOT$/path/virtual/foo.h", @"$ROOT$/real/foo.h");
    }

    [Test]
    public void Unicode()
    {
        var contents =
            "{\n"
            + "  'version': 0,\n"
            + "  'roots': [\n"
            + "    {\n"
            + "      'type': 'directory',\n"
            + "      'name': \"$ROOT$/path/\\u266B\",\n"
            + "      'contents': [\n"
            + "        {\n"
            + "          'type': 'file',\n"
            + "          'name': \"\\u2602.h\",\n"
            + "          'external-contents': \"$ROOT$/real/\\u2602.h\"\n"
            + "        }\n"
            + "      ]\n"
            + "    }\n"
            + "  ]\n"
            + "}\n";

        using var t = new TestVFO(contents);
        t.Map("$ROOT$/path/♫/☂.h", "$ROOT$/real/☂.h");
    }

    [Test]
    public void InvalidArgs()
    {
        using var t = new TestVFO(null);
        t.MapError("$ROOT$/path/./virtual/../foo.h", "$ROOT$/real/foo.h", CXError_InvalidArguments);
    }

    [Test]
    public void RemapDirectories()
    {
        var contents =
            "{\n"
            + "  'version': 0,\n"
            + "  'roots': [\n"
            + "    {\n"
            + "      'type': 'directory',\n"
            + "      'name': \"$ROOT$/another/dir\",\n"
            + "      'contents': [\n"
            + "        {\n"
            + "          'type': 'file',\n"
            + "          'name': \"foo2.h\",\n"
            + "          'external-contents': \"$ROOT$/real/foo2.h\"\n"
            + "        }\n"
            + "      ]\n"
            + "    },\n"
            + "    {\n"
            + "      'type': 'directory',\n"
            + "      'name': \"$ROOT$/path/virtual/dir\",\n"
            + "      'contents': [\n"
            + "        {\n"
            + "          'type': 'file',\n"
            + "          'name': \"foo1.h\",\n"
            + "          'external-contents': \"$ROOT$/real/foo1.h\"\n"
            + "        },\n"
            + "        {\n"
            + "          'type': 'file',\n"
            + "          'name': \"foo3.h\",\n"
            + "          'external-contents': \"$ROOT$/real/foo3.h\"\n"
            + "        },\n"
            + "        {\n"
            + "          'type': 'directory',\n"
            + "          'name': \"in/subdir\",\n"
            + "          'contents': [\n"
            + "            {\n"
            + "              'type': 'file',\n"
            + "              'name': \"foo4.h\",\n"
            + "              'external-contents': \"$ROOT$/real/foo4.h\"\n"
            + "            }\n"
            + "          ]\n"
            + "        }\n"
            + "      ]\n"
            + "    }\n"
            + "  ]\n"
            + "}\n";

        using var t = new TestVFO(contents);
        t.Map("$ROOT$/path/virtual/dir/foo1.h", "$ROOT$/real/foo1.h");
        t.Map("$ROOT$/another/dir/foo2.h", "$ROOT$/real/foo2.h");
        t.Map("$ROOT$/path/virtual/dir/foo3.h", "$ROOT$/real/foo3.h");
        t.Map("$ROOT$/path/virtual/dir/in/subdir/foo4.h", "$ROOT$/real/foo4.h");
    }

    [Test]
    public void CaseInsensitive()
    {
        var contents =
            "{\n"
            + "  'version': 0,\n"
            + "  'case-sensitive': 'false',\n"
            + "  'roots': [\n"
            + "    {\n"
            + "      'type': 'directory',\n"
            + "      'name': \"$ROOT$/path/virtual\",\n"
            + "      'contents': [\n"
            + "        {\n"
            + "          'type': 'file',\n"
            + "          'name': \"foo.h\",\n"
            + "          'external-contents': \"$ROOT$/real/foo.h\"\n"
            + "        }\n"
            + "      ]\n"
            + "    }\n"
            + "  ]\n"
            + "}\n";

        using var t = new TestVFO(contents);
        t.Map("$ROOT$/path/virtual/foo.h", "$ROOT$/real/foo.h");
        _ = t.VFO.SetCaseSensitivity(false);
    }

    [Test]
    public void SharedPrefix()
    {
        var contents =
            "{\n"
            + "  'version': 0,\n"
            + "  'roots': [\n"
            + "    {\n"
            + "      'type': 'directory',\n"
            + "      'name': \"$ROOT$/path/foo\",\n"
            + "      'contents': [\n"
            + "        {\n"
            + "          'type': 'file',\n"
            + "          'name': \"bar\",\n"
            + "          'external-contents': \"$ROOT$/real/bar\"\n"
            + "        },\n"
            + "        {\n"
            + "          'type': 'file',\n"
            + "          'name': \"bar.h\",\n"
            + "          'external-contents': \"$ROOT$/real/bar.h\"\n"
            + "        }\n"
            + "      ]\n"
            + "    },\n"
            + "    {\n"
            + "      'type': 'directory',\n"
            + "      'name': \"$ROOT$/path/fooBar\",\n"
            + "      'contents': [\n"
            + "        {\n"
            + "          'type': 'file',\n"
            + "          'name': \"baz.h\",\n"
            + "          'external-contents': \"$ROOT$/real/baz.h\"\n"
            + "        }\n"
            + "      ]\n"
            + "    },\n"
            + "    {\n"
            + "      'type': 'directory',\n"
            + "      'name': \"$ROOT$/path\",\n"
            + "      'contents': [\n"
            + "        {\n"
            + "          'type': 'file',\n"
            + "          'name': \"fooBarBaz.h\",\n"
            + "          'external-contents': \"$ROOT$/real/fooBarBaz.h\"\n"
            + "        }\n"
            + "      ]\n"
            + "    }\n"
            + "  ]\n"
            + "}\n";

        using var t = new TestVFO(contents);
        t.Map("$ROOT$/path/foo/bar.h", "$ROOT$/real/bar.h");
        t.Map("$ROOT$/path/foo/bar", "$ROOT$/real/bar");
        t.Map("$ROOT$/path/fooBar/baz.h", "$ROOT$/real/baz.h");
        t.Map("$ROOT$/path/fooBarBaz.h", "$ROOT$/real/fooBarBaz.h");
    }

    [Test]
    public void AdjacentDirectory()
    {
        var contents =
            "{\n"
            + "  'version': 0,\n"
            + "  'roots': [\n"
            + "    {\n"
            + "      'type': 'directory',\n"
            + "      'name': \"$ROOT$/path/dir1\",\n"
            + "      'contents': [\n"
            + "        {\n"
            + "          'type': 'file',\n"
            + "          'name': \"foo.h\",\n"
            + "          'external-contents': \"$ROOT$/real/foo.h\"\n"
            + "        },\n"
            + "        {\n"
            + "          'type': 'directory',\n"
            + "          'name': \"subdir\",\n"
            + "          'contents': [\n"
            + "            {\n"
            + "              'type': 'file',\n"
            + "              'name': \"bar.h\",\n"
            + "              'external-contents': \"$ROOT$/real/bar.h\"\n"
            + "            }\n"
            + "          ]\n"
            + "        }\n"
            + "      ]\n"
            + "    },\n"
            + "    {\n"
            + "      'type': 'directory',\n"
            + "      'name': \"$ROOT$/path/dir2\",\n"
            + "      'contents': [\n"
            + "        {\n"
            + "          'type': 'file',\n"
            + "          'name': \"baz.h\",\n"
            + "          'external-contents': \"$ROOT$/real/baz.h\"\n"
            + "        }\n"
            + "      ]\n"
            + "    }\n"
            + "  ]\n"
            + "}\n";

        using var t = new TestVFO(contents);
        t.Map("$ROOT$/path/dir1/foo.h", "$ROOT$/real/foo.h");
        t.Map("$ROOT$/path/dir1/subdir/bar.h", "$ROOT$/real/bar.h");
        t.Map("$ROOT$/path/dir2/baz.h", "$ROOT$/real/baz.h");
    }

    [Test]
    public void TopLevel()
    {
        var contents =
            "{\n"
            + "  'version': 0,\n"
            + "  'roots': [\n"
            + "    {\n"
            + "      'type': 'directory',\n"
            + "      'name': \"$ROOT$/\",\n"
            + "      'contents': [\n"
            + "        {\n"
            + "          'type': 'file',\n"
            + "          'name': \"foo.h\",\n"
            + "          'external-contents': \"$ROOT$/real/foo.h\"\n"
            + "        }\n"
            + "      ]\n"
            + "    }\n"
            + "  ]\n"
            + "}\n";

        using var t = new TestVFO(contents);
        t.Map("$ROOT$/foo.h", "$ROOT$/real/foo.h");
    }

    [Test]
    public void Empty()
    {
        var contents =
            "{\n"
            + "  'version': 0,\n"
            + "  'roots': [\n"
            + "  ]\n"
            + "}\n";

        using var t = new TestVFO(contents);
    }
}
