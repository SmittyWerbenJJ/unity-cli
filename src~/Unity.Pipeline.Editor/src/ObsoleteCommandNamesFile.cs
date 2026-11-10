using System;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;
using UnityEngine;

namespace Unity.Pipeline.Editor
{
    /// <summary>
    /// The flat names commands had before they were organised in groups, each mapped to the full
    /// name that replaced it. Read from <c>ObsoleteCommandNames.json</c> beside this file.
    /// </summary>
    static class ObsoleteCommandNamesFile
    {
        const string FileName = "ObsoleteCommandNames.json";

        /// <summary>
        /// Reads the map from the package. Call it from the main thread: locating the package
        /// goes through the Package Manager.
        /// </summary>
        /// <returns>Old name to full name, or null when the file cannot be read.</returns>
        internal static IReadOnlyDictionary<string, string> Load()
        {
#if UNITY_2019_2_OR_NEWER
            var package = UnityEditor.PackageManager.PackageInfo.FindForAssembly(typeof(ObsoleteCommandNamesFile).Assembly);
            if (package == null || string.IsNullOrEmpty(package.resolvedPath))
                return null;

            var path = Path.Combine(package.resolvedPath, "Editor", FileName);
#else
            var path = Path.GetFullPath("Assets/com.unity.pipeline/Editor/" + FileName);
            if (!File.Exists(path))
                path = Path.GetFullPath("Packages/com.unity.pipeline/Editor/" + FileName);
            if (!File.Exists(path))
                return null;
#endif
            try
            {
                return JsonConvert.DeserializeObject<Dictionary<string, string>>(File.ReadAllText(path));
            }
            catch (Exception ex)
            {
                Debug.LogError($"Pipeline: could not read {path}: {ex.Message}");
                return null;
            }
        }
    }
}
