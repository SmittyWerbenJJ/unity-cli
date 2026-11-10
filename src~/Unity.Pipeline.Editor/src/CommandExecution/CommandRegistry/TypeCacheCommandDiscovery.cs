using System;
using System.Collections.Generic;
using System.Reflection;
using Unity.Pipeline.Commands;
using UnityEditor;

namespace Unity.Pipeline.Editor
{
    /// <summary>
    /// Editor implementation of command discovery using Unity's TypeCache.
    /// Provides fast command discovery in Editor mode via Unity's optimized caching system.
    /// </summary>
    class TypeCacheCommandDiscovery : ICommandDiscovery
    {
        /// <summary>
        /// Find all methods marked with specified attribute using TypeCache.
        /// </summary>
        /// <typeparam name="T">The attribute type to search for.</typeparam>
        /// <returns>The matching methods.</returns>
        public IEnumerable<MethodInfo> GetMethodsWithAttribute<T>() where T : Attribute
        {
#if UNITY_2019_2_OR_NEWER
            return TypeCache.GetMethodsWithAttribute<T>();
#else
            var result = new List<MethodInfo>();
            foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
            {
                var name = assembly.FullName;
                if (name.StartsWith("System") || name.StartsWith("mscorlib") || name.StartsWith("Mono"))
                    continue;
                try
                {
                    foreach (var type in assembly.GetTypes())
                    {
                        foreach (var method in type.GetMethods(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.Instance))
                        {
                            if (method.IsDefined(typeof(T), false))
                                result.Add(method);
                        }
                    }
                }
                catch { }
            }
            return result;
#endif
        }
    }
}