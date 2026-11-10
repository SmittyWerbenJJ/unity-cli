using System;
using UnityEngine;
using UnityEditor;
using Unity.Pipeline;
using Object = UnityEngine.Object;

namespace Unity.Pipeline.Adapters
{
    /// <summary>
    /// Common startup registrar that binds the version-appropriate adapter into UnityAdapterBridge.
    /// </summary>
    [UnityEditor.InitializeOnLoad]
    public static class UnityAdapterStartup
    {
        static UnityAdapterStartup()
        {
#if UNITY_2018_1_OR_NEWER && !UNITY_2019_1_OR_NEWER
            UnityAdapterBridge.Current = new Unity2018Adapter();
#elif UNITY_2019_1_OR_NEWER && !UNITY_2020_1_OR_NEWER
            UnityAdapterBridge.Current = new Unity2019Adapter();
#elif UNITY_2020_1_OR_NEWER && !UNITY_6000_0_OR_NEWER
            UnityAdapterBridge.Current = new Unity2022Adapter();
#elif UNITY_6000_0_OR_NEWER
            UnityAdapterBridge.Current = new Unity6000Adapter();
#endif

            UnityAdapterBridge.Current.RegisterReloadLifecycle(
                onBeforeReload: () => { },
                onQuitting: () => { }
            );

            UnityAdapterBridge.Current.HookToolbar(() =>
            {
                Debug.Log($"[Pipeline] Server active on {UnityAdapterBridge.Current.TargetVersion}");
            });
        }
    }
}
