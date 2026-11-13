using System;
using System.IO;
using System.Reflection;

namespace Unity.Pipeline.Editor
{
    /// <summary>
    /// Resolves the on-disk root directory of the package containing <see cref="Unity.Pipeline.Editor"/>
    /// across Unity 2018.1 through 6000+ without hardcoding a specific package name or install method.
    /// </summary>
    internal static class PackagePathResolver
    {
        internal static string ResolvePackageRoot(Assembly assembly = null)
        {
            assembly = assembly ?? typeof(PackagePathResolver).Assembly;

            // 1. Unity 2019.2+: PackageManager.PackageInfo.FindForAssembly(assembly).resolvedPath
            var packageInfoType = Type.GetType("UnityEditor.PackageManager.PackageInfo, UnityEditor");
            if (packageInfoType != null)
            {
                var findMethod = packageInfoType.GetMethod(
                    "FindForAssembly",
                    BindingFlags.Public | BindingFlags.Static,
                    null,
                    new[] { typeof(Assembly) },
                    null);
                if (findMethod != null)
                {
                    var info = findMethod.Invoke(null, new object[] { assembly });
                    if (info != null)
                    {
                        var resolvedProp = packageInfoType.GetProperty("resolvedPath", BindingFlags.Public | BindingFlags.Instance);
                        var resolvedPath = resolvedProp?.GetValue(info) as string;
                        if (!string.IsNullOrEmpty(resolvedPath) && Directory.Exists(resolvedPath))
                            return resolvedPath;
                    }
                }
            }

            // 2. Fallback (Unity 2018.1 or shadow-copied assemblies): derive from assembly location (.../Editor/<dll>)
            var location = assembly.Location;
            if (!string.IsNullOrEmpty(location))
            {
                var editorDir = Path.GetDirectoryName(location);
                var rootDir = editorDir != null ? Path.GetDirectoryName(editorDir) : null;
                if (!string.IsNullOrEmpty(rootDir) && File.Exists(Path.Combine(rootDir, "package.json")))
                    return rootDir;
            }

            // 3. Last-resort scan of Assets/ and Packages/ for any folder containing our Editor DLL
            var dllName = assembly.GetName().Name + ".dll";
            foreach (var parent in new[] { "Packages", "Assets" })
            {
                var parentFull = Path.GetFullPath(parent);
                if (!Directory.Exists(parentFull))
                    continue;

                foreach (var dir in Directory.GetDirectories(parentFull))
                {
                    if (File.Exists(Path.Combine(dir, "Editor", dllName)))
                        return dir;
                }
            }

            return null;
        }
    }
}
