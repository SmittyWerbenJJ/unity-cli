#if UNITY_2020_1_OR_NEWER && !UNITY_6000_0_OR_NEWER
using System;
using System.Reflection;
using UnityEngine;
using UnityEditor;
using Unity.Pipeline;
using Object = UnityEngine.Object;

namespace Unity.Pipeline.Adapters
{
    /// <summary>
    /// Version adapter for Unity 2022.3 LTS.
    /// Uses modern GameObject.FindObjectsByType, AssetDatabase.SaveAssetIfDirty,
    /// and 2022 SceneManagement PrefabStage.
    /// </summary>
    public class Unity2022Adapter : IUnityAdapter
    {
        public string TargetVersion => "2022.3";

        public T[] FindObjects<T>() where T : Object
        {
#if UNITY_2021_3_OR_NEWER
            return GameObject.FindObjectsByType<T>(FindObjectsSortMode.None);
#else
            return Object.FindObjectsOfType<T>();
#endif
        }

        public void SaveAsset(Object asset)
        {
            if (asset != null)
            {
#if UNITY_2020_3_OR_NEWER
                UnityEditor.AssetDatabase.SaveAssetIfDirty(asset);
#else
                UnityEditor.EditorUtility.SetDirty(asset);
                UnityEditor.AssetDatabase.SaveAssets();
#endif
            }
        }

        public void CaptureScreen(RenderTexture rt)
        {
            if (rt == null) return;
            ScreenCapture.CaptureScreenshotIntoRenderTexture(rt);
        }

        public object GetCurrentPrefabStage()
        {
            try
            {
                var stageType = typeof(UnityEditor.Editor).Assembly.GetType("UnityEditor.SceneManagement.PrefabStageUtility");
                var method = stageType?.GetMethod("GetCurrentPrefabStage", BindingFlags.Public | BindingFlags.Static);
                return method?.Invoke(null, null);
            }
            catch
            {
                return null;
            }
        }

        public void HookToolbar(Action onToggle)
        {
            try
            {
                var toolbarType = typeof(UnityEditor.Editor).Assembly.GetType("UnityEditor.Toolbar");
                var getMethod = toolbarType?.GetMethod("get", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static);
                var toolbarInstance = getMethod?.Invoke(null, null);
            }
            catch { }
        }

        public void RegisterReloadLifecycle(Action onBeforeReload, Action onQuitting)
        {
            if (onBeforeReload != null)
                UnityEditor.AssemblyReloadEvents.beforeAssemblyReload += () => onBeforeReload();
            if (onQuitting != null)
                UnityEditor.EditorApplication.quitting += () => onQuitting();
        }

        public bool IsBuildProfileSupported() => false;

        public object FallbackExecute(string commandName, object[] args)
        {
            return new
            {
                success = false,
                error = "UNSUPPORTED_IN_EDITOR_VERSION",
                message = $"Command '{commandName}' is unsupported in Unity 2022.3 LTS."
            };
        }
    }
}
#endif
