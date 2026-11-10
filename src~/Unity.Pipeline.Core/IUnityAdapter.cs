using System;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Unity.Pipeline
{
    /// <summary>
    /// Uniform contract interface implemented by all version-specific Unity adapters
    /// (unity_2018.cs, unity_2019.cs, unity_2022.cs, unity_6000.cs).
    /// Implementation differences are completely encapsulated within each adapter.
    /// </summary>
    public interface IUnityAdapter
    {
        /// <summary>Target Unity version string for reporting (e.g. "2018.1", "2019.4", "2022.3", "6000.0").</summary>
        string TargetVersion { get; }

        /// <summary>Search for loaded objects of type T in the active domain.</summary>
        T[] FindObjects<T>() where T : Object;

        /// <summary>Mark an asset dirty and persist changes to disk.</summary>
        void SaveAsset(Object asset);

        /// <summary>Capture editor or game view output into a RenderTexture.</summary>
        void CaptureScreen(RenderTexture rt);

        /// <summary>Retrieve the current PrefabStage instance, or null if unsupported/closed.</summary>
        object GetCurrentPrefabStage();

        /// <summary>Register top toolbar icon/toggle hook.</summary>
        void HookToolbar(Action onToggle);

        /// <summary>Hook engine lifecycle events for domain reload and shutdown cleanup.</summary>
        void RegisterReloadLifecycle(Action onBeforeReload, Action onQuitting);

        /// <summary>Whether Build Profiles (Unity 6+) are supported in this engine instance.</summary>
        bool IsBuildProfileSupported();

        /// <summary>Unified fallback dispatcher for commands unsupported in this engine version.</summary>
        object FallbackExecute(string commandName, object[] args);
    }

    /// <summary>
    /// Global registry bridge that links the active version adapter to the Core DLL at runtime.
    /// </summary>
    public static class UnityAdapterBridge
    {
        private static IUnityAdapter s_ActiveAdapter;

        /// <summary>The active version adapter for this Editor instance.</summary>
        public static IUnityAdapter Current
        {
            get => s_ActiveAdapter ?? (s_ActiveAdapter = new DefaultFallbackAdapter());
            set => s_ActiveAdapter = value;
        }

        private class DefaultFallbackAdapter : IUnityAdapter
        {
            public string TargetVersion => "Universal";
            public T[] FindObjects<T>() where T : Object => Object.FindObjectsOfType<T>();
            public void SaveAsset(Object asset) { }
            public void CaptureScreen(RenderTexture rt) { }
            public object GetCurrentPrefabStage() => null;
            public void HookToolbar(Action onToggle) { }
            public void RegisterReloadLifecycle(Action onBeforeReload, Action onQuitting) { }
            public bool IsBuildProfileSupported() => false;
            public object FallbackExecute(string commandName, object[] args) => null;
        }
    }
}
