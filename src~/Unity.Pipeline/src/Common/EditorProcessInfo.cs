#if UNITY_EDITOR
using System;
using UnityEditor;

namespace Unity.Pipeline
{
    /// <summary>
    /// Facts about the current Editor process. In the Runtime assembly so runtime code running in the
    /// Editor can use them too.
    /// </summary>
    static class EditorProcessInfo
    {
        /// <summary>
        /// Whether this is the Editor process that owns the project, as opposed to an asset import
        /// worker or another secondary process.
        /// </summary>
        /// <returns>True in the main Editor process.</returns>
        internal static bool IsMainProcess()
        {
            // A read-only or virtual project clone (e.g. Multiplayer Play Mode) owns its own project.
            foreach (var arg in Environment.GetCommandLineArgs())
            {
                if (arg == "-readonly" || arg == "--virtual-project-clone")
                    return true;
            }

            try
            {
                var isWorkerMethod = typeof(AssetDatabase).GetMethod("IsAssetImportWorkerProcess", System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Static);
                if (isWorkerMethod != null && (bool)isWorkerMethod.Invoke(null, null))
                    return false;

                var mpeType = typeof(Editor).Assembly.GetType("UnityEditor.MPE.ProcessService");
                if (mpeType != null)
                {
                    var levelProp = mpeType.GetProperty("level", System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Static);
                    if (levelProp != null)
                    {
                        var val = levelProp.GetValue(null);
                        return val != null && val.ToString() == "Main";
                    }
                }
            }
            catch { }

            return true;
        }

        /// <summary>
        /// Whether this Editor is a virtual project clone, such as a Multiplayer Play Mode virtual
        /// player. A clone shares the main Editor's project settings, so it must not bind the HTTP
        /// port the main Editor already owns (UUM-151576).
        /// </summary>
        /// <returns>True in a clone Editor.</returns>
        internal static bool IsVirtualProjectClone()
        {
#if UNITY_6000_5_OR_NEWER
            return !Unity.Multiplayer.PlayMode.CurrentPlayer.IsMainEditor;
#else
            return IsVirtualProjectClone(Environment.GetCommandLineArgs());
#endif
        }

        /// <summary>
        /// Command-line fallback for <see cref="IsVirtualProjectClone()"/> before 6000.5, where
        /// <c>CurrentPlayer</c> throws when Multiplayer Play Mode has no player data.
        /// </summary>
        /// <param name="commandLineArgs">The process's command line arguments.</param>
        /// <returns>True if <c>--virtual-project-clone</c> is one of them.</returns>
        internal static bool IsVirtualProjectClone(string[] commandLineArgs)
        {
            foreach (var arg in commandLineArgs)
            {
                if (arg == "--virtual-project-clone")
                    return true;
            }
            return false;
        }

        /// <summary>
        /// Whether the Editor was launched with -automated. A Unity console warning for this fact
        /// is unactionable (nobody watches the console in an agent-driven workflow) and, worse, was
        /// logged once per constructed server instance — the isolated test servers built per test
        /// (see TestEditorPipelineServer) spammed it dozens of times per suite run. The fact is
        /// instead surfaced as structured data on the instance descriptor (see
        /// <see cref="PipelineDescriptorPublisher"/>) — the one thing every client reads before it can
        /// connect at all (UUM-149977).
        /// </summary>
        /// <param name="commandLineArgs">The process's command line arguments.</param>
        /// <returns>True if <c>-automated</c> is one of them.</returns>
        internal static bool IsAutomated(string[] commandLineArgs)
        {
            foreach (var arg in commandLineArgs)
            {
                if (arg == "-automated")
                    return true;
            }
            return false;
        }
    }
}
#endif
