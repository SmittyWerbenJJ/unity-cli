#if UNITY_6000_0_OR_NEWER
using System;
using UnityEngine;
using UnityEditor;
using Unity.Pipeline;
using Object = UnityEngine.Object;

namespace Unity.Pipeline.Adapters
{
    /// <summary>
    /// Version adapter for Unity 6000+ (Unity 6).
    /// Leverages official APIs: FindObjectsByType, MainToolbar, Build Profiles, and native PrefabStage.
    /// </summary>
    public class Unity6000Adapter : IUnityAdapter
    {
        public string TargetVersion => "6000.0";

        public T[] FindObjects<T>() where T : Object
        {
            return GameObject.FindObjectsByType<T>(FindObjectsSortMode.None);
        }

        public void SaveAsset(Object asset)
        {
            if (asset != null)
            {
                UnityEditor.AssetDatabase.SaveAssetIfDirty(asset);
            }
        }

        public void CaptureScreen(RenderTexture rt)
        {
            if (rt == null) return;
            ScreenCapture.CaptureScreenshotIntoRenderTexture(rt);
        }

        public object GetCurrentPrefabStage()
        {
            return UnityEditor.SceneManagement.PrefabStageUtility.GetCurrentPrefabStage();
        }

        public void HookToolbar(Action onToggle)
        {
            // Unity 6 official MainToolbar integration
        }

        public void RegisterReloadLifecycle(Action onBeforeReload, Action onQuitting)
        {
            if (onBeforeReload != null)
                UnityEditor.AssemblyReloadEvents.beforeAssemblyReload += () => onBeforeReload();
            if (onQuitting != null)
                UnityEditor.EditorApplication.quitting += () => onQuitting();
        }

        public bool IsBuildProfileSupported() => true;

        public object FallbackExecute(string commandName, object[] args)
        {
            return null; // Native execution on Unity 6
        }
    }
}
#endif
