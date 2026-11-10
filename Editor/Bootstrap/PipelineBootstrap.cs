using System;
using System.IO;
using System.IO.Compression;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace Unity.Pipeline.Bootstrap
{
    /// <summary>
    /// First-init zero-touch bootstrapper for Unity Pipeline.
    /// Detects Unity version and checks if Newtonsoft.Json already exists and works in the project.
    /// If missing (e.g., on Unity 2018 where UPM com.unity.nuget.newtonsoft-json is unavailable),
    /// unpacks the bundled Newtonsoft.Json.zip into Assets/Plugins/Newtonsoft.Json.dll.
    /// Written in strict C# 6.0 with zero external assembly references so Unity 2018.1 mcs.exe compiles it cleanly.
    /// </summary>
    [InitializeOnLoad]
    internal static class PipelineBootstrap
    {
        private const string TargetRelativePath = "Assets/Plugins/Newtonsoft.Json.dll";
        private const string ZipFileName = "Newtonsoft.Json.zip";
        private static Assembly s_loadedNewtonsoft;

        static PipelineBootstrap()
        {
            AppDomain.CurrentDomain.AssemblyResolve -= OnAssemblyResolve;
            AppDomain.CurrentDomain.AssemblyResolve += OnAssemblyResolve;

            try
            {
                EnsureNewtonsoftJson();
            }
            catch (Exception ex)
            {
                Debug.LogWarning("[Unity.Pipeline] Bootstrap check failed: " + ex.Message);
            }
        }

        private static Assembly OnAssemblyResolve(object sender, ResolveEventArgs args)
        {
            if (args.Name != null && args.Name.StartsWith("Newtonsoft.Json", StringComparison.OrdinalIgnoreCase))
            {
                if (s_loadedNewtonsoft != null)
                {
                    return s_loadedNewtonsoft;
                }
                string projectRoot = Path.GetDirectoryName(Application.dataPath);
                string targetFullPath = Path.Combine(projectRoot, TargetRelativePath.Replace('/', Path.DirectorySeparatorChar));
                if (File.Exists(targetFullPath))
                {
                    try
                    {
                        s_loadedNewtonsoft = Assembly.LoadFrom(targetFullPath);
                        return s_loadedNewtonsoft;
                    }
                    catch
                    {
                    }
                }
            }
            return null;
        }

        [MenuItem("Window/Unity Pipeline/Verify Dependencies")]
        public static void EnsureNewtonsoftJson()
        {
            // 1. Check if Newtonsoft.Json is already loaded and working in the current AppDomain
            if (IsNewtonsoftWorking())
            {
                return;
            }

            // 2. Check if Newtonsoft.Json.dll already exists anywhere under Assets/
            string assetsDir = Application.dataPath;
            if (Directory.Exists(assetsDir))
            {
                string[] existing = Directory.GetFiles(assetsDir, "Newtonsoft.Json.dll", SearchOption.AllDirectories);
                if (existing != null && existing.Length > 0)
                {
                    if (CanLoadAndUseNewtonsoft(existing[0]))
                    {
                        return;
                    }
                }
            }

            // 3. On Unity 2019+ UPM normally provides com.unity.nuget.newtonsoft-json,
            //    but if it is still missing (such as Unity 2018.x), extract from bundled Newtonsoft.Json.zip.
            string zipPath = FindBundledZipPath();
            if (string.IsNullOrEmpty(zipPath) || !File.Exists(zipPath))
            {
                return;
            }

            string projectRoot = Path.GetDirectoryName(assetsDir);
            string targetFullPath = Path.Combine(projectRoot, TargetRelativePath.Replace('/', Path.DirectorySeparatorChar));
            string targetDir = Path.GetDirectoryName(targetFullPath);
            if (!Directory.Exists(targetDir))
            {
                Directory.CreateDirectory(targetDir);
            }

            if (ExtractFirstEntryFromZip(zipPath, targetFullPath))
            {
                Debug.Log("[Unity.Pipeline] Unpacked Newtonsoft.Json.dll to " + TargetRelativePath + " for Unity " + Application.unityVersion);
                CanLoadAndUseNewtonsoft(targetFullPath);
                TryStartPipelineServer();
                EditorApplication.delayCall += delegate
                {
                    if (!Application.isBatchMode)
                    {
                        AssetDatabase.ImportAsset(TargetRelativePath, ImportAssetOptions.ForceUpdate);
                    }
                };
            }
        }

        private static void TryStartPipelineServer()
        {
            try
            {
                Assembly[] assemblies = AppDomain.CurrentDomain.GetAssemblies();
                for (int i = 0; i < assemblies.Length; i++)
                {
                    if (assemblies[i].GetName().Name == "Unity.Pipeline.Editor")
                    {
                        Type startup = assemblies[i].GetType("Unity.Pipeline.Editor.PipelineServerStartup", false);
                        if (startup != null)
                        {
                            MethodInfo startMethod = startup.GetMethod("StartServer", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static);
                            if (startMethod != null)
                            {
                                startMethod.Invoke(null, null);
                            }
                        }
                        break;
                    }
                }
            }
            catch
            {
            }
        }

        private static bool IsNewtonsoftWorking()
        {
            try
            {
                Assembly[] assemblies = AppDomain.CurrentDomain.GetAssemblies();
                for (int i = 0; i < assemblies.Length; i++)
                {
                    Type jsonConvert = assemblies[i].GetType("Newtonsoft.Json.JsonConvert", false);
                    if (jsonConvert != null)
                    {
                        MethodInfo serialize = jsonConvert.GetMethod("SerializeObject", new Type[] { typeof(object) });
                        if (serialize != null)
                        {
                            object res = serialize.Invoke(null, new object[] { "ok" });
                            if (res as string == "\"ok\"")
                            {
                                return true;
                            }
                        }
                    }
                }
            }
            catch
            {
            }
            return false;
        }

        private static bool CanLoadAndUseNewtonsoft(string dllPath)
        {
            try
            {
                s_loadedNewtonsoft = Assembly.LoadFrom(dllPath);
                Type jsonConvert = s_loadedNewtonsoft.GetType("Newtonsoft.Json.JsonConvert", false);
                if (jsonConvert != null)
                {
                    MethodInfo serialize = jsonConvert.GetMethod("SerializeObject", new Type[] { typeof(object) });
                    if (serialize != null)
                    {
                        object res = serialize.Invoke(null, new object[] { "ok" });
                        return (res as string) == "\"ok\"";
                    }
                }
            }
            catch
            {
            }
            return false;
        }

        private static string FindBundledZipPath()
        {
            string projectRoot = Path.GetDirectoryName(Application.dataPath);
            string[] roots = new string[]
            {
                Path.Combine(projectRoot, "Packages"),
                Application.dataPath
            };

            for (int r = 0; r < roots.Length; r++)
            {
                if (Directory.Exists(roots[r]))
                {
                    try
                    {
                        string[] matches = Directory.GetFiles(roots[r], ZipFileName, SearchOption.AllDirectories);
                        if (matches != null && matches.Length > 0)
                        {
                            return matches[0];
                        }
                    }
                    catch
                    {
                    }
                }
            }
            return null;
        }

        private static bool ExtractFirstEntryFromZip(string zipPath, string outputPath)
        {
            using (FileStream fs = new FileStream(zipPath, FileMode.Open, FileAccess.Read, FileShare.Read))
            using (BinaryReader reader = new BinaryReader(fs))
            {
                uint signature = reader.ReadUInt32();
                if (signature != 0x04034b50U) // "PK\x03\x04"
                {
                    return false;
                }

                reader.ReadUInt16(); // version needed
                ushort flags = reader.ReadUInt16();
                ushort method = reader.ReadUInt16();
                reader.ReadUInt16(); // mod time
                reader.ReadUInt16(); // mod date
                reader.ReadUInt32(); // crc32
                uint compressedSize = reader.ReadUInt32();
                uint uncompressedSize = reader.ReadUInt32();
                ushort fileNameLen = reader.ReadUInt16();
                ushort extraLen = reader.ReadUInt16();

                reader.ReadBytes(fileNameLen);
                if (extraLen > 0)
                {
                    reader.ReadBytes(extraLen);
                }

                if ((flags & 0x0008) != 0 && (compressedSize == 0 || uncompressedSize == 0))
                {
                    return false;
                }

                byte[] compressedData = reader.ReadBytes((int)compressedSize);

                if (method == 0) // Store
                {
                    File.WriteAllBytes(outputPath, compressedData);
                    return true;
                }
                else if (method == 8) // Deflate
                {
                    using (MemoryStream ms = new MemoryStream(compressedData))
                    using (DeflateStream deflate = new DeflateStream(ms, CompressionMode.Decompress))
                    using (FileStream outFs = new FileStream(outputPath, FileMode.Create, FileAccess.Write, FileShare.None))
                    {
                        byte[] buffer = new byte[8192];
                        int read;
                        while ((read = deflate.Read(buffer, 0, buffer.Length)) > 0)
                        {
                            outFs.Write(buffer, 0, read);
                        }
                    }
                    return true;
                }
            }
            return false;
        }
    }
}
