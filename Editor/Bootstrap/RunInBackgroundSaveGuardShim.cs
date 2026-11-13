using System;
using System.Reflection;

#if UNITY_2019_1_OR_NEWER
namespace Unity.Pipeline.Bootstrap
{
    /// <summary>
    /// Source-compiled AssetModificationProcessor shim for Unity 2019.1+ (where the base class
    /// lives in UnityEditor) forwarding to the precompiled Unity.Pipeline.Editor guard.
    /// </summary>
    internal class RunInBackgroundSaveGuardShim : UnityEditor.AssetModificationProcessor
    {
        private static MethodInfo s_onWillSave;

        internal static string[] OnWillSaveAssets(string[] paths)
        {
            return RunInBackgroundSaveGuardForwarder.Forward(ref s_onWillSave, paths);
        }
    }
}
#else
namespace Unity.Pipeline.Bootstrap
{
    /// <summary>
    /// Source-compiled AssetModificationProcessor shim for Unity 2018.x (where the base class
    /// lives in the global namespace) forwarding to the precompiled Unity.Pipeline.Editor guard.
    /// </summary>
    internal class RunInBackgroundSaveGuardShim : global::AssetModificationProcessor
    {
        private static MethodInfo s_onWillSave;

        internal static string[] OnWillSaveAssets(string[] paths)
        {
            return RunInBackgroundSaveGuardForwarder.Forward(ref s_onWillSave, paths);
        }
    }
}
#endif

namespace Unity.Pipeline.Bootstrap
{
    internal static class RunInBackgroundSaveGuardForwarder
    {
        internal static string[] Forward(ref MethodInfo cachedMethod, string[] paths)
        {
            try
            {
                if (cachedMethod == null)
                {
                    Assembly[] assemblies = AppDomain.CurrentDomain.GetAssemblies();
                    for (int i = 0; i < assemblies.Length; i++)
                    {
                        if (assemblies[i].GetName().Name == "Unity.Pipeline.Editor")
                        {
                            Type guard = assemblies[i].GetType("Unity.Pipeline.Editor.RunInBackgroundSaveGuard", false);
                            if (guard != null)
                            {
                                cachedMethod = guard.GetMethod("OnWillSaveAssets", BindingFlags.Public | BindingFlags.Static);
                            }
                            break;
                        }
                    }
                }

                if (cachedMethod != null)
                {
                    object result = cachedMethod.Invoke(null, new object[] { paths });
                    string[] updated = result as string[];
                    if (updated != null)
                    {
                        return updated;
                    }
                }
            }
            catch
            {
            }

            return paths;
        }
    }
}
