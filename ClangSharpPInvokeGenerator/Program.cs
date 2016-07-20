namespace ClangSharpPInvokeGenerator
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Text.RegularExpressions;
    using ClangSharp;

    public class Program
    {
        public static void Main(string[] args)
        {
            Regex re = new Regex(@"(?<switch>-{1,2}\S*)(?:[=:]?|\s+)(?<value>[^-\s].*?)?(?=\s+[-]|$)");
            List<KeyValuePair<string, string>> matches = (from match in re.Matches(string.Join(" ", args)).Cast<Match>()
                select new KeyValuePair<string, string>(match.Groups["switch"].Value, match.Groups["value"].Value))
                .ToList();

            var files = new List<string>();
            var includeDirs = new List<string>();
            string outputFile = string.Empty;
            string @namespace = string.Empty;
            string libraryPath = string.Empty;
            string prefixStrip = string.Empty;
            string methodClassName = "Methods";

            foreach (KeyValuePair<string, string> match in matches)
            {
                if (string.Equals(match.Key, "--n") || string.Equals(match.Key, "--namespace"))
                {
                    @namespace = match.Value;
                }

                if (string.Equals(match.Key, "--l") || string.Equals(match.Key, "--libraryPath"))
                {
                    libraryPath = match.Value;
                }

                if (string.Equals(match.Key, "--i") || string.Equals(match.Key, "--include"))
                {
                    includeDirs.Add(match.Value);
                }

                if (string.Equals(match.Key, "--o") || string.Equals(match.Key, "--output"))
                {
                    outputFile = match.Value;
                }

                if (string.Equals(match.Key, "--f") || string.Equals(match.Key, "--file"))
                {
                    files.Add(match.Value);
                }

                if (string.Equals(match.Key, "--p") || string.Equals(match.Key, "--prefixStrip"))
                {
                    prefixStrip = match.Value;
                }

                if (string.Equals(match.Key, "--m") || string.Equals(match.Key, "--methodClassName"))
                {
                    methodClassName = match.Value;
                }
            }

            var errorList = new List<string>();
            if (!files.Any())
            {
                errorList.Add("Error: No input C/C++ files provided. Use --file or --f");
            }

            if (string.IsNullOrWhiteSpace(@namespace))
            {
                errorList.Add("Error: No namespace provided. Use --namespace or --n");
            }

            if (string.IsNullOrWhiteSpace(outputFile))
            {
                errorList.Add("Error: No output file location provided. Use --output or --o");
            }

            if (string.IsNullOrWhiteSpace(libraryPath))
            {
                errorList.Add("Error: No library path location provided. Use --libraryPath or --l");
            }

            if (errorList.Any())
            {
                Console.WriteLine("Usage: ClangPInvokeGenerator --file [fileLocation] --libraryPath [library.dll] --output [output.cs] --namespace [Namespace] --include [headerFileIncludeDirs]");
                foreach (var error in errorList)
                {
                    Console.WriteLine(error);
                }
            }

            var createIndex = clang.createIndex(0, 0);
            string[] arr = { "-x", "c++" };
            
            arr = arr.Concat(includeDirs.Select(x => "-I" + x)).ToArray();

            List<CXTranslationUnit> translationUnits = new List<CXTranslationUnit>();

            foreach (var file in files)
            {
                CXTranslationUnit translationUnit;
                CXUnsavedFile[] unsavedFile = new CXUnsavedFile[0];
                var translationUnitError = clang.parseTranslationUnit2(createIndex, file, arr, 3, unsavedFile, 0, 0, out translationUnit);
                
                if (translationUnitError != CXErrorCode.CXError_Success)
                {
                    Console.WriteLine("Error: " + translationUnitError);
                    var numDiagnostics = clang.getNumDiagnostics(translationUnit);

                    for (uint i = 0; i < numDiagnostics; ++i)
                    {
                        var diagnostic = clang.getDiagnostic(translationUnit, i);
                        Console.WriteLine(clang.getDiagnosticSpelling(diagnostic).ToString());
                        clang.disposeDiagnostic(diagnostic);
                    }
                }

                translationUnits.Add(translationUnit);
            }

            using (var sw = new StreamWriter(outputFile))
            {
                sw.WriteLine("namespace " + @namespace);
                sw.WriteLine("{");

                sw.WriteLine("    using System;");
                sw.WriteLine("    using System.Runtime.InteropServices;");
                sw.WriteLine();

                var structVisitor = new StructVisitor(sw);
                foreach (var tu in translationUnits)
                {
                    clang.visitChildren(clang.getTranslationUnitCursor(tu), structVisitor.Visit, new CXClientData(IntPtr.Zero));
                }

                var typeDefVisitor = new TypeDefVisitor(sw);
                foreach (var tu in translationUnits)
                {
                    clang.visitChildren(clang.getTranslationUnitCursor(tu), typeDefVisitor.Visit, new CXClientData(IntPtr.Zero));
                }
                
                var enumVisitor = new EnumVisitor(sw);
                foreach (var tu in translationUnits)
                {
                    clang.visitChildren(clang.getTranslationUnitCursor(tu), enumVisitor.Visit, new CXClientData(IntPtr.Zero));
                }

                sw.WriteLine("    public static partial class " + methodClassName);
                sw.WriteLine("    {");
                {
                    var functionVisitor = new FunctionVisitor(sw, libraryPath, prefixStrip);
                    foreach (var tu in translationUnits)
                    {
                        clang.visitChildren(clang.getTranslationUnitCursor(tu), functionVisitor.Visit, new CXClientData(IntPtr.Zero));
                    }
                }
                sw.WriteLine("    }");
                sw.WriteLine("}");
            }

            foreach (var tu in translationUnits)
            {
                clang.disposeTranslationUnit(tu);
            }

            clang.disposeIndex(createIndex);
        }
    }
}