#if UNITY_2018_1_OR_NEWER && !UNITY_2019_1_OR_NEWER
using System;
using System.Reflection;
using UnityEngine;
using UnityEditor;
using Unity.Pipeline;
using Object = UnityEngine.Object;

namespace Unity.Pipeline.Adapters
{
    /// <summary>
    /// Version adapter for Unity 2018.1.
    /// Handles C# 7 language level, legacy AssetDatabase, camera blit captures,
    /// and reflection-based toolbar hooks.
    /// </summary>
    public class Unity2018Adapter : IUnityAdapter
    {
        public string TargetVersion => "2018.1";

        public T[] FindObjects<T>() where T : Object
        {
            return Object.FindObjectsOfType<T>();
        }

        public void SaveAsset(Object asset)
        {
            if (asset != null)
            {
                UnityEditor.EditorUtility.SetDirty(asset);
                UnityEditor.AssetDatabase.SaveAssets();
            }
        }

        public void CaptureScreen(RenderTexture rt)
        {
            if (rt == null) return;
            var cam = Camera.main;
            if (cam != null)
            {
                var prev = cam.targetTexture;
                cam.targetTexture = rt;
                cam.Render();
                cam.targetTexture = prev;
            }
        }

        public object GetCurrentPrefabStage()
        {
            // Prefab Mode was introduced in Unity 2018.3. Null indicates no active stage.
            return null;
        }

        public void HookToolbar(Action onToggle)
        {
            try
            {
                var toolbarType = typeof(UnityEditor.Editor).Assembly.GetType("UnityEditor.Toolbar");
                if (toolbarType != null)
                {
                    var getMethod = toolbarType.GetMethod("get", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static);
                    var toolbarInstance = getMethod?.Invoke(null, null);
                }
            }
            catch (Exception ex)
            {
                Debug.LogWarning("[Pipeline] Toolbar hook reflection: " + ex.Message);
            }
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
                message = $"Command '{commandName}' is unsupported in Unity 2018.1."
            };
        }
    }
}
#endif
