// Copyright (c) .NET Foundation and Contributors. All Rights Reserved. Licensed under the MIT License (MIT). See License.md in the repository root for more information.

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.IO;
using ClangSharp.Interop;

namespace ClangSharp;

internal static partial class Program
{
    // The Clang major version the tool is built against. The resource directory is versioned by
    // this (lib/clang/<major>) and the builtin headers are only guaranteed to be compatible with a
    // matching major version, so any detected directory must line up with it.
    private static int ClangMajorVersion => clang.MajorVersion;

    // Mirrors what the clang driver does automatically: locate the resource directory holding the
    // builtin headers (stddef.h, stdarg.h, the intrinsics, ...) and pass it via -resource-dir so
    // users don't have to wire it up by hand. Unlike the driver we cannot derive it relative to our
    // own binary -- the headers are not shipped with libClang -- so we probe for an installed and
    // version-matched Clang instead.
    private static string[] AddResourceDirectory(string[] clangCommandLineArgs, IReadOnlyList<string> additionalArgs)
    {
        // Only auto-detect on Unix. There, nothing but the compiler provides the freestanding
        // headers (stddef.h, stdarg.h, the intrinsics, ...), so a missing resource dir is fatal.
        // Windows has a fallback: the MSVC/Windows SDK toolchain ships compatible copies, so clang
        // resolves them even without a resource dir. An explicit --resource-directory is still
        // honored on every platform.
        var detect = (!s_resourceDirectoryDetectionDisabled.IsPresent && !OperatingSystem.IsWindows())
                   ? DetectClangResourceDirectory
                   : (Func<string?>?)null;

        if (TryResolveResourceDirectory(s_resourceDirectory.IsPresent ? s_resourceDirectory.SingleValue : null,
                                        additionalArgs,
                                        detect,
                                        out var resourceDirectory,
                                        out var warning))
        {
            clangCommandLineArgs = [.. clangCommandLineArgs, "-resource-dir", resourceDirectory];
        }

        if (warning is not null)
        {
            Console.Error.WriteLine(warning);
        }

        return clangCommandLineArgs;
    }

    // Decides which resource directory (if any) should be injected. Kept free of any environment
    // access -- the probe is supplied via `detect` (null when auto-detection doesn't apply) -- so
    // the precedence rules can be unit tested.
    internal static bool TryResolveResourceDirectory(string? explicitResourceDirectory, IReadOnlyList<string> additionalArgs, Func<string?>? detect, [NotNullWhen(true)] out string? resourceDirectory, out string? warning)
    {
        warning = null;

        // An explicit --resource-directory always wins and is honored as-is. We deliberately don't
        // fail when it looks wrong so the caller stays in control of their own toolchain.
        if (!string.IsNullOrWhiteSpace(explicitResourceDirectory))
        {
            resourceDirectory = explicitResourceDirectory;
            return true;
        }

        // Respect a -resource-dir passed through --additional; don't second-guess it or add another.
        if (AdditionalArgsSpecifyResourceDirectory(additionalArgs))
        {
            resourceDirectory = null;
            return false;
        }

        // Auto-detection is off (disabled by the user or not applicable to this platform).
        if (detect is null)
        {
            resourceDirectory = null;
            return false;
        }

        resourceDirectory = detect();

        if (resourceDirectory is not null)
        {
            return true;
        }

        warning = $"Warning: No Clang resource directory was found; builtin headers such as 'stddef.h' may fail to resolve. " +
                  $"Specify one with --resource-directory <dir>, install LLVM/Clang {ClangMajorVersion}, or pass --no-resource-directory-detection to silence this.";
        return false;
    }

    private static bool AdditionalArgsSpecifyResourceDirectory(IReadOnlyList<string> additionalArgs)
    {
        foreach (var arg in additionalArgs)
        {
            if (arg.Equals("-resource-dir", StringComparison.Ordinal) ||
                arg.Equals("--resource-dir", StringComparison.Ordinal) ||
                arg.StartsWith("-resource-dir=", StringComparison.Ordinal) ||
                arg.StartsWith("--resource-dir=", StringComparison.Ordinal))
            {
                return true;
            }
        }

        return false;
    }

    private static string? DetectClangResourceDirectory()
    {
        foreach (var candidate in EnumerateResourceDirectoryCandidates())
        {
            if (IsValidResourceDirectory(candidate) && HasMatchingMajorVersion(candidate))
            {
                return candidate;
            }
        }

        return null;
    }

    private static IEnumerable<string> EnumerateResourceDirectoryCandidates()
    {
        var major = ClangMajorVersion.ToString(CultureInfo.InvariantCulture);

        // 1. Next to the tool / native libClang, in case a resource directory is shipped side-by-side.
        foreach (var baseDirectory in EnumerateApplicationDirectories())
        {
            yield return Path.Combine(baseDirectory, "lib", "clang", major);
        }

        // 2. Ask a Clang on PATH where its resource directory lives.
        if (TryGetResourceDirectoryFromClang(out var fromClang))
        {
            yield return fromClang;
        }

        // 3. Well-known install locations for the current operating system.
        foreach (var candidate in EnumerateWellKnownResourceDirectories(major))
        {
            yield return candidate;
        }
    }

    private static IEnumerable<string> EnumerateApplicationDirectories()
    {
        var seen = new HashSet<string>(StringComparer.Ordinal);

        // Environment.ProcessPath is the launching executable; AppContext.BaseDirectory is the
        // assembly directory. They differ when the tool is hosted, so probe both.
        var processDirectory = (Environment.ProcessPath is string processPath) ? Path.GetDirectoryName(processPath) : null;

        if (!string.IsNullOrEmpty(processDirectory) && seen.Add(processDirectory))
        {
            yield return processDirectory;
        }

        var baseDirectory = AppContext.BaseDirectory;

        if (!string.IsNullOrEmpty(baseDirectory))
        {
            baseDirectory = baseDirectory.TrimEnd(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar);

            if (seen.Add(baseDirectory))
            {
                yield return baseDirectory;
            }
        }
    }

    // Only reached on Unix; auto-detection is not wired up on Windows (see AddResourceDirectory).
    private static IEnumerable<string> EnumerateWellKnownResourceDirectories(string major)
    {
        if (OperatingSystem.IsMacOS())
        {
            // Homebrew is the common source of an upstream (non-Apple) Clang on macOS; Apple's own
            // Clang uses a different versioning scheme and is filtered out by the major check.
            yield return Path.Combine("/opt/homebrew/opt/llvm/lib/clang", major);
            yield return Path.Combine("/usr/local/opt/llvm/lib/clang", major);
        }
        else
        {
            yield return Path.Combine($"/usr/lib/llvm-{major}/lib/clang", major);
            yield return Path.Combine("/usr/lib/clang", major);
            yield return Path.Combine("/usr/lib64/clang", major);
            yield return Path.Combine("/usr/local/lib/clang", major);
        }
    }

    private static bool TryGetResourceDirectoryFromClang([NotNullWhen(true)] out string? resourceDirectory)
    {
        string[] clangExecutables = [$"clang-{ClangMajorVersion.ToString(CultureInfo.InvariantCulture)}", "clang"];

        foreach (var clangExecutable in clangExecutables)
        {
            if (TryReadProcessOutput(clangExecutable, "-print-resource-dir", out var output) && !string.IsNullOrWhiteSpace(output))
            {
                resourceDirectory = output.Trim();
                return true;
            }
        }

        resourceDirectory = null;
        return false;
    }

    // A directory is only usable if it actually contains the builtin headers we care about.
    internal static bool IsValidResourceDirectory(string? resourceDirectory)
    {
        return !string.IsNullOrEmpty(resourceDirectory)
            && File.Exists(Path.Combine(resourceDirectory, "include", "stddef.h"));
    }

    // The builtin headers can reference builtins that only exist in a matching compiler, so we only
    // accept a directory whose lib/clang/<major> segment matches the libClang we ship against.
    internal static bool HasMatchingMajorVersion(string? resourceDirectory)
    {
        if (string.IsNullOrEmpty(resourceDirectory))
        {
            return false;
        }

        var name = Path.GetFileName(resourceDirectory.TrimEnd(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar));
        var dotIndex = name.IndexOf('.', StringComparison.Ordinal);

        if (dotIndex >= 0)
        {
            name = name[..dotIndex];
        }

        return int.TryParse(name, NumberStyles.None, CultureInfo.InvariantCulture, out var major)
            && (major == ClangMajorVersion);
    }

    private static bool TryReadProcessOutput(string fileName, string arguments, out string output)
    {
        output = "";

        try
        {
            using var process = new Process {
                StartInfo = new ProcessStartInfo(fileName, arguments) {
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    UseShellExecute = false,
                    CreateNoWindow = true,
                },
            };

            if (!process.Start())
            {
                return false;
            }

            // The output is a single short path, so reading it fully before waiting cannot deadlock.
            output = process.StandardOutput.ReadToEnd();

            if (!process.WaitForExit(milliseconds: 5000))
            {
                return false;
            }

            return process.ExitCode == 0;
        }
        catch (Win32Exception)
        {
            // The executable was not found on PATH.
            return false;
        }
        catch (InvalidOperationException)
        {
            return false;
        }
        catch (IOException)
        {
            return false;
        }
    }
}
