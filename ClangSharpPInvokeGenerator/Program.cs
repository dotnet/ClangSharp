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
            var defines = new List<string>();
            var additionalArgs = new List<string>();
            string outputFile = string.Empty;
            string @namespace = string.Empty;
            string libraryPath = string.Empty;
            string prefixStrip = string.Empty;
            string methodClassName = "Methods";
            string excludeFunctions = string.Empty;
            string[] excludeFunctionsArray = null;

            foreach (KeyValuePair<string, string> match in matches)
            {
                if (string.Equals(match.Key, "--a") || string.Equals(match.Key, "--additional"))
                {
                    additionalArgs.Add(match.Value);
                }

                if (string.Equals(match.Key, "--d") || string.Equals(match.Key, "--define"))
                {
                    defines.Add(match.Value);
                }

                if (string.Equals(match.Key, "--e") || string.Equals(match.Key, "--excludeFunctions"))
                {
                    excludeFunctions = match.Value;
                }

                if (string.Equals(match.Key, "--f") || string.Equals(match.Key, "--file"))
                {
                    files.Add(match.Value);
                }

                if (string.Equals(match.Key, "--i") || string.Equals(match.Key, "--include"))
                {
                    includeDirs.Add(match.Value);
                }

                if (string.Equals(match.Key, "--l") || string.Equals(match.Key, "--libraryPath"))
                {
                    libraryPath = match.Value;
                }

                if (string.Equals(match.Key, "--m") || string.Equals(match.Key, "--methodClassName"))
                {
                    methodClassName = match.Value;
                }

                if (string.Equals(match.Key, "--n") || string.Equals(match.Key, "--namespace"))
                {
                    @namespace = match.Value;
                }

                if (string.Equals(match.Key, "--o") || string.Equals(match.Key, "--output"))
                {
                    outputFile = match.Value;
                }

                if (string.Equals(match.Key, "--p") || string.Equals(match.Key, "--prefixStrip"))
                {
                    prefixStrip = match.Value;
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
                Console.WriteLine("Usage: ClangPInvokeGenerator --file [fileLocation] --libraryPath [library.dll] --output [output.cs] --namespace [Namespace] --include [headerFileIncludeDirs] --define [compilerDefine] --additional [compilerArg] --excludeFunctions [func1,func2]");
                foreach (var error in errorList)
                {
                    Console.WriteLine(error);
                }
            }

            if (!string.IsNullOrEmpty(excludeFunctions))
            {
                excludeFunctionsArray = excludeFunctions.Split(',').Select(x => x.Trim()).ToArray();
            }

            var createIndex = CXIndex.Create();
            string[] arr = { "-x", "c++" };

            arr = arr.Concat(includeDirs.Select(x => "-I" + x)).ToArray();
            arr = arr.Concat(defines.Select(x => "-D" + x)).ToArray();
            arr = arr.Concat(additionalArgs).ToArray();

            List<CXTranslationUnit> translationUnits = new List<CXTranslationUnit>();

            foreach (var file in files)
            {
                CXTranslationUnit translationUnit;
                CXUnsavedFile[] unsavedFile = new CXUnsavedFile[0];
                var translationUnitError = CXTranslationUnit.Parse(createIndex, file, arr, unsavedFile, CXTranslationUnit_Flags.CXTranslationUnit_None, out translationUnit);

                if (translationUnitError != CXErrorCode.CXError_Success)
                {
                    Console.WriteLine($"Error: '{translationUnitError}' for '{file}'.");
                    var numDiagnostics = translationUnit.NumDiagnostics;

                    for (uint i = 0; i < numDiagnostics; ++i)
                    {
                        var diagnostic = translationUnit.GetDiagnostic(i);
                        Console.WriteLine(diagnostic.Spelling.ToString());
                        diagnostic.Dispose();
                    }
                }

                translationUnits.Add(translationUnit);
            }

            using (var sw = new StreamWriter(outputFile))
            {
                sw.NewLine = "\n";

                sw.WriteLine("namespace " + @namespace);
                sw.WriteLine("{");

                sw.WriteLine("    using System;");
                sw.WriteLine("    using System.Runtime.InteropServices;");
                sw.WriteLine();

                var structDeclWriter = new CursorWriter(sw, indentation: 1, (cursor) => cursor.Kind == CXCursorKind.CXCursor_StructDecl);
                foreach (var tu in translationUnits)
                {
                    tu.Cursor.VisitChildren(structDeclWriter.VisitTranslationUnit, clientData: default);
                }

                var typedefDeclWriter = new CursorWriter(sw, indentation: 1, (cursor) => cursor.Kind == CXCursorKind.CXCursor_TypedefDecl);
                foreach (var tu in translationUnits)
                {
                    tu.Cursor.VisitChildren(typedefDeclWriter.VisitTranslationUnit, clientData: default);
                }
                
                var enumDeclWriter = new CursorWriter(sw, indentation: 1, (cursor) => cursor.Kind == CXCursorKind.CXCursor_EnumDecl);
                foreach (var tu in translationUnits)
                {
                    tu.Cursor.VisitChildren(enumDeclWriter.VisitTranslationUnit, new CXClientData(IntPtr.Zero));
                }
                
                sw.WriteLine("    public static partial class " + methodClassName);
                sw.WriteLine("    {");
                {
                    var functionDeclWriter = new CursorWriter(sw, indentation: 2, (cursor) => cursor.Kind == CXCursorKind.CXCursor_FunctionDecl);

                    sw.WriteLine($"        private const string libraryPath = \"{libraryPath}\";");
                    sw.WriteLine();

                    functionDeclWriter.PrefixStrip = prefixStrip;
                    functionDeclWriter.ExcludeFunctionsArray = excludeFunctionsArray ?? Array.Empty<string>();

                    foreach (var tu in translationUnits)
                    {
                        tu.Cursor.VisitChildren(functionDeclWriter.VisitTranslationUnit, new CXClientData(IntPtr.Zero));
                    }
                }
                sw.WriteLine("    }");
                sw.WriteLine("}");
            }

            foreach (var tu in translationUnits)
            {
                tu.Dispose();
            }

            createIndex.Dispose();
        }
    }
}
