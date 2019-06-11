using System;
using System.Runtime.InteropServices;
using Xunit;

namespace ClangSharp.UnitTests
{
    // NOTE: To make those tests work on both Windows and *nix,
    //       begin any absolute path with "$ROOT$", which the TestVFO infra will replace
    //       with either "C:" (Windows) or nothing (*nix) so that clang understands the path.
    //       (Technically, /a/path/like/this is a path relative to the root of the current drive
    //        on Windows, but clang doesn't seem to support this)
    public class VirtualFileOverlay
    {
        class TestVFO : IDisposable
        {
            public CXVirtualFileOverlay VFO;

            private string Contents;
            private bool isDisposed = false;

            public TestVFO(string contents)
            {
                VFO = CXVirtualFileOverlay.Create(options: 0);
                Contents = Fix(contents);
            }

            ~TestVFO()
            {
                Dispose(isDisposing: false);
            }

            public void Map(string vPath, string rPath)
            {
                vPath = Fix(vPath);
                rPath = Fix(rPath);

                var err = VFO.AddFileMapping(vPath, rPath);
                Assert.Equal(CXErrorCode.CXError_Success, err);
            }

            public void MapError(string vPath, string rPath, CXErrorCode expErr)
            {
                vPath = Fix(vPath);
                rPath = Fix(rPath);

                var err = VFO.AddFileMapping(vPath, rPath);
                Assert.Equal(expErr, err);
            }

            private string Fix(string text)
            {
                if (text is null)
                {
                    return null;
                }

                var replacement = RuntimeInformation.IsOSPlatform(OSPlatform.Windows) ? "C:" : "";
                return text.Replace("$ROOT$", replacement);
            }

            public void Dispose()
            {
                Dispose(true);
                GC.SuppressFinalize(this);
            }

            protected virtual void Dispose(bool isDisposing)
            {
                if (isDisposed)
                {
                    return;
                }
                isDisposed = true;

                if (Contents != null)
                {
                    Span<byte> buffer = VFO.WriteToBuffer(options: 0, errorCode: out _);
                    Assert.Equal(Contents, buffer.AsString());
                    buffer.ClangFree();
                }

                VFO.Dispose();
            }
        }

        [Fact]
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

            using (TestVFO T = new TestVFO(contents))
            {
                T.Map(@"$ROOT$/path/virtual/foo.h", @"$ROOT$/real/foo.h");
            }
        }

        [Fact]
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

            using (TestVFO T = new TestVFO(contents))
            {
                T.Map("$ROOT$/path/♫/☂.h", "$ROOT$/real/☂.h");
            }
        }

        [Fact]
        public void InvalidArgs()
        {
            using (TestVFO T = new TestVFO(null))
            {
                T.MapError("$ROOT$/path/./virtual/../foo.h", "$ROOT$/real/foo.h",
                           CXErrorCode.CXError_InvalidArguments);
            }
        }

        [Fact]
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

            using (TestVFO T = new TestVFO(contents))
            {
                T.Map("$ROOT$/path/virtual/dir/foo1.h", "$ROOT$/real/foo1.h");
                T.Map("$ROOT$/another/dir/foo2.h", "$ROOT$/real/foo2.h");
                T.Map("$ROOT$/path/virtual/dir/foo3.h", "$ROOT$/real/foo3.h");
                T.Map("$ROOT$/path/virtual/dir/in/subdir/foo4.h", "$ROOT$/real/foo4.h");
            }
        }

        [Fact]
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

            using (TestVFO T = new TestVFO(contents))
            {
                T.Map("$ROOT$/path/virtual/foo.h", "$ROOT$/real/foo.h");
                T.VFO.SetCaseSensitivity(false);
            }
        }

        [Fact]
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
                + "      'name': \"$ROOT$/path/foobar\",\n"
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
                + "          'name': \"foobarbaz.h\",\n"
                + "          'external-contents': \"$ROOT$/real/foobarbaz.h\"\n"
                + "        }\n"
                + "      ]\n"
                + "    }\n"
                + "  ]\n"
                + "}\n";

            using (TestVFO T = new TestVFO(contents))
            {
                T.Map("$ROOT$/path/foo/bar.h", "$ROOT$/real/bar.h");
                T.Map("$ROOT$/path/foo/bar", "$ROOT$/real/bar");
                T.Map("$ROOT$/path/foobar/baz.h", "$ROOT$/real/baz.h");
                T.Map("$ROOT$/path/foobarbaz.h", "$ROOT$/real/foobarbaz.h");
            }
        }

        [Fact]
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

            using (TestVFO T = new TestVFO(contents))
            {
                T.Map("$ROOT$/path/dir1/foo.h", "$ROOT$/real/foo.h");
                T.Map("$ROOT$/path/dir1/subdir/bar.h", "$ROOT$/real/bar.h");
                T.Map("$ROOT$/path/dir2/baz.h", "$ROOT$/real/baz.h");
            }
        }

        [Fact]
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

            using (TestVFO T = new TestVFO(contents))
            {
                T.Map("$ROOT$/foo.h", "$ROOT$/real/foo.h");
            }
        }

        [Fact]
        public void Empty()
        {
            var contents =
                "{\n"
                + "  'version': 0,\n"
                + "  'roots': [\n"
                + "  ]\n"
                + "}\n";

            using (TestVFO T = new TestVFO(contents))
            {
            }
        }
    }
}
