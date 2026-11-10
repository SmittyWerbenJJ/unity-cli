using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using Newtonsoft.Json;
using Unity.Pipeline.Commands;
using UnityEditor;
using UnityEngine;

namespace Unity.Pipeline.Editor.Commands
{
    /// <summary>
    /// IvanMurzak/Unity-MCP compatibility commands.
    /// Implements high-value non-superseded features from IvanMurzak/Unity-MCP:
    /// 1. execute_menu_item: Menu item execution with safety blacklists and direct MCP tool compatibility.
    /// 2. unity_reflect / reflect: Live C# reflection inspector for types, methods, properties, and assembly search.
    /// 3. get_sha: Asset/file SHA256 checksum computation for concurrency and cache verification.
    /// 4. find_in_file: Project-wide text/regex search across source and asset files.
    /// 5. apply_text_edits: Targeted text replacement with precondition SHA256 validation.
    /// 6. gameobject duplicate: Scene GameObject duplication preserving hierarchy and local transforms.
    /// 7. scene frame: SceneView camera framing on target GameObjects.
    /// Conforms strictly to C# 7.3 and .NET Standard 2.0.
    /// </summary>
    public static class IvanMurzakCompatCommands
    {
        #region 1. Execute Menu Item

        private static readonly HashSet<string> s_MenuBlacklist = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
        {
            "File/Quit",
            "File/Exit"
        };

        [CliCommand("execute_menu_item", "Execute a Unity Editor menu item by path with safety blacklist guards (IvanMurzak/Unity-MCP compatibility)", MainThreadRequired = true, Tags = new[] { "editor", "compat" })]
        public static MenuItemResponse ExecuteMenuItem(
            [CliArg("menu_path", "Menu item path to execute, e.g. 'Assets/Reimport All'")] string menu_path = "")
        {
            if (string.IsNullOrWhiteSpace(menu_path))
            {
                return new MenuItemResponse
                {
                    success = false,
                    message = "Required parameter 'menu_path' is missing or empty."
                };
            }

            string trimmedPath = menu_path.Trim();
            if (s_MenuBlacklist.Contains(trimmedPath))
            {
                return new MenuItemResponse
                {
                    success = false,
                    path = trimmedPath,
                    message = "Execution of menu item '" + trimmedPath + "' is blocked for safety reasons (use pipeline editor quit command instead)."
                };
            }

            bool executed = false;
            try
            {
                executed = EditorApplication.ExecuteMenuItem(trimmedPath);
            }
            catch (Exception ex)
            {
                return new MenuItemResponse
                {
                    success = false,
                    path = trimmedPath,
                    message = "Exception executing menu item '" + trimmedPath + "': " + ex.Message
                };
            }

            if (!executed)
            {
                return new MenuItemResponse
                {
                    success = false,
                    path = trimmedPath,
                    message = "Failed to execute menu item '" + trimmedPath + "'. It may be invalid, disabled, or context-dependent."
                };
            }

            return new MenuItemResponse
            {
                success = true,
                path = trimmedPath,
                message = "Executed menu item '" + trimmedPath + "' successfully."
            };
        }

        public class MenuItemResponse
        {
            public bool success;
            public string path;
            public string message;
        }

        #endregion

        #region 2. Reflection Inspector (unity_reflect / reflect)

        private static readonly Dictionary<Type, string> s_FriendlyTypeNames = new Dictionary<Type, string>
        {
            { typeof(void), "void" },
            { typeof(int), "int" },
            { typeof(float), "float" },
            { typeof(bool), "bool" },
            { typeof(string), "string" },
            { typeof(double), "double" },
            { typeof(long), "long" },
            { typeof(object), "object" },
            { typeof(byte), "byte" },
            { typeof(short), "short" },
            { typeof(char), "char" },
            { typeof(decimal), "decimal" },
            { typeof(uint), "uint" },
            { typeof(ulong), "ulong" },
            { typeof(ushort), "ushort" },
            { typeof(sbyte), "sbyte" }
        };

        [CliCommand("unity_reflect", "Inspect Unity's live C# API via reflection: get_type, get_member, or search (IvanMurzak compatibility)", MainThreadRequired = false, Tags = new[] { "reflect", "docs", "compat" })]
        public static object UnityReflectUnified(
            [CliArg("action", "Reflection action: 'get_type', 'get_member', or 'search'")] string action,
            [CliArg("class_name", "Target type name, e.g. 'UnityEngine.Camera' or 'Transform'")] string class_name = "",
            [CliArg("member_name", "Member name to inspect for 'get_member'")] string member_name = "",
            [CliArg("query", "Search string for 'search' action")] string query = "",
            [CliArg("scope", "Assembly filter scope: 'unity' (UnityEngine/UnityEditor), 'project' (Assembly-CSharp*), or 'all'")] string scope = "unity",
            [CliArg("max_results", "Maximum types/members to return (default 50)")] int max_results = 50)
        {
            if (string.IsNullOrWhiteSpace(action))
                return new { success = false, message = "'action' parameter is required ('get_type', 'get_member', or 'search')." };

            string normAction = action.Trim().ToLowerInvariant();
            switch (normAction)
            {
                case "get_type":
                    return ReflectGetType(class_name);
                case "get_member":
                    return ReflectGetMember(class_name, member_name);
                case "search":
                    return ReflectSearch(query, scope, max_results);
                default:
                    return new { success = false, message = "Unknown action '" + action + "'. Valid actions: get_type, get_member, search." };
            }
        }

        [CliCommand("reflect", "search", "Search for types across loaded Unity assemblies by name query (substring or wildcard)", MainThreadRequired = false, Tags = new[] { "reflect", "compat" })]
        public static object ReflectSearch(
            [CliArg("query", "Type name or substring to search for (e.g. 'CinemachineBrain', 'NavMeshAgent')")] string query,
            [CliArg("scope", "Assembly filter scope: 'unity' (UnityEngine/UnityEditor), 'project' (Assembly-CSharp*), or 'all'")] string scope = "unity",
            [CliArg("max_results", "Maximum number of types to return (default 50)")] int max_results = 50)
        {
            if (string.IsNullOrWhiteSpace(query))
                return new { success = false, message = "'query' parameter is required." };

            if (max_results <= 0)
                max_results = 50;

            string q = query.Trim();
            bool isWildcard = q.Contains("*") || q.Contains("?");
            Regex regex = null;
            if (isWildcard)
            {
                string pat = "^" + Regex.Escape(q).Replace("\\*", ".*").Replace("\\?", ".") + "$";
                regex = new Regex(pat, RegexOptions.IgnoreCase);
            }

            List<Type> allTypes = GetLoadedTypes(scope);
            List<object> results = new List<object>();

            for (int i = 0; i < allTypes.Count; i++)
            {
                Type t = allTypes[i];
                if (!t.IsPublic && !t.IsNestedPublic)
                    continue;

                bool match = false;
                if (regex != null)
                {
                    match = regex.IsMatch(t.Name) || (t.FullName != null && regex.IsMatch(t.FullName));
                }
                else
                {
                    match = t.Name.IndexOf(q, StringComparison.OrdinalIgnoreCase) >= 0 ||
                            (t.FullName != null && t.FullName.IndexOf(q, StringComparison.OrdinalIgnoreCase) >= 0);
                }

                if (match)
                {
                    results.Add(new
                    {
                        name = t.Name,
                        full_name = t.FullName ?? t.Name,
                        @namespace = t.Namespace ?? "",
                        assembly = t.Assembly.GetName().Name,
                        is_class = t.IsClass,
                        is_enum = t.IsEnum,
                        is_interface = t.IsInterface,
                        is_value_type = t.IsValueType
                    });

                    if (results.Count >= max_results)
                        break;
                }
            }

            return new
            {
                success = true,
                query = q,
                scope = scope,
                count = results.Count,
                types = results
            };
        }

        [CliCommand("reflect", "get_type", "Get summary of all public methods, properties, fields, and events for a type", MainThreadRequired = false, Tags = new[] { "reflect", "compat" })]
        public static object ReflectGetType(
            [CliArg("class_name", "Full or simple type name, e.g. 'UnityEngine.Camera' or 'Transform'")] string class_name)
        {
            if (string.IsNullOrWhiteSpace(class_name))
                return new { success = false, message = "'class_name' parameter is required." };

            Type targetType = ResolveType(class_name);
            if (targetType == null)
            {
                return new
                {
                    success = false,
                    message = "Type '" + class_name + "' could not be found across loaded assemblies."
                };
            }

            BindingFlags flags = BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static;

            List<string> methods = targetType.GetMethods(flags)
                .Where(m => !m.IsSpecialName)
                .Select(m => m.Name)
                .Distinct()
                .OrderBy(n => n)
                .ToList();

            List<string> properties = targetType.GetProperties(flags)
                .Select(p => p.Name)
                .Distinct()
                .OrderBy(n => n)
                .ToList();

            List<string> fields = targetType.GetFields(flags)
                .Select(f => f.Name)
                .Distinct()
                .OrderBy(n => n)
                .ToList();

            List<string> events = targetType.GetEvents(flags)
                .Select(e => e.Name)
                .Distinct()
                .OrderBy(n => n)
                .ToList();

            return new
            {
                success = true,
                name = targetType.Name,
                full_name = targetType.FullName ?? targetType.Name,
                @namespace = targetType.Namespace ?? "",
                assembly = targetType.Assembly.GetName().Name,
                base_type = targetType.BaseType != null ? FormatTypeName(targetType.BaseType) : null,
                is_class = targetType.IsClass,
                is_enum = targetType.IsEnum,
                is_interface = targetType.IsInterface,
                is_value_type = targetType.IsValueType,
                methods = methods,
                properties = properties,
                fields = fields,
                events = events
            };
        }

        [CliCommand("reflect", "get_member", "Get detailed signature, parameter types, and return type for a specific member", MainThreadRequired = false, Tags = new[] { "reflect", "compat" })]
        public static object ReflectGetMember(
            [CliArg("class_name", "Full or simple type name, e.g. 'UnityEngine.Camera'")] string class_name,
            [CliArg("member_name", "Member name to inspect, e.g. 'ScreenToWorldPoint' or 'main'")] string member_name)
        {
            if (string.IsNullOrWhiteSpace(class_name) || string.IsNullOrWhiteSpace(member_name))
                return new { success = false, message = "Both 'class_name' and 'member_name' are required." };

            Type targetType = ResolveType(class_name);
            if (targetType == null)
                return new { success = false, message = "Type '" + class_name + "' could not be found." };

            string memName = member_name.Trim();
            BindingFlags flags = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static;
            MemberInfo[] members = targetType.GetMember(memName, flags);

            if (members == null || members.Length == 0)
            {
                return new
                {
                    success = false,
                    message = "Member '" + memName + "' not found on type '" + targetType.FullName + "'."
                };
            }

            List<object> details = new List<object>();
            for (int i = 0; i < members.Length; i++)
            {
                MemberInfo mi = members[i];
                if (mi is MethodInfo method)
                {
                    ParameterInfo[] pars = method.GetParameters();
                    List<object> parList = new List<object>();
                    for (int p = 0; p < pars.Length; p++)
                    {
                        ParameterInfo pi = pars[p];
                        parList.Add(new
                        {
                            name = pi.Name,
                            type = FormatTypeName(pi.ParameterType),
                            is_optional = pi.IsOptional,
                            has_default = pi.HasDefaultValue,
                            default_value = pi.HasDefaultValue ? (pi.DefaultValue == null ? "null" : pi.DefaultValue.ToString()) : null
                        });
                    }

                    details.Add(new
                    {
                        member_type = "Method",
                        name = method.Name,
                        return_type = FormatTypeName(method.ReturnType),
                        parameters = parList,
                        is_static = method.IsStatic,
                        is_public = method.IsPublic,
                        is_virtual = method.IsVirtual,
                        is_abstract = method.IsAbstract
                    });
                }
                else if (mi is PropertyInfo prop)
                {
                    details.Add(new
                    {
                        member_type = "Property",
                        name = prop.Name,
                        property_type = FormatTypeName(prop.PropertyType),
                        can_read = prop.CanRead,
                        can_write = prop.CanWrite,
                        is_static = (prop.GetGetMethod(true) ?? prop.GetSetMethod(true))?.IsStatic ?? false
                    });
                }
                else if (mi is FieldInfo field)
                {
                    details.Add(new
                    {
                        member_type = "Field",
                        name = field.Name,
                        field_type = FormatTypeName(field.FieldType),
                        is_static = field.IsStatic,
                        is_constant = field.IsLiteral
                    });
                }
                else if (mi is EventInfo evt)
                {
                    details.Add(new
                    {
                        member_type = "Event",
                        name = evt.Name,
                        event_handler_type = FormatTypeName(evt.EventHandlerType)
                    });
                }
            }

            return new
            {
                success = true,
                class_name = targetType.FullName,
                member_name = memName,
                count = details.Count,
                signatures = details
            };
        }

        private static List<Type> GetLoadedTypes(string scope)
        {
            string s = (scope ?? "unity").ToLowerInvariant().Trim();
            Assembly[] assemblies = AppDomain.CurrentDomain.GetAssemblies();
            List<Type> types = new List<Type>();

            for (int i = 0; i < assemblies.Length; i++)
            {
                Assembly asm = assemblies[i];
                string asmName = asm.GetName().Name;

                bool include = false;
                if (s == "all")
                {
                    include = true;
                }
                else if (s == "project")
                {
                    include = asmName.StartsWith("Assembly-CSharp", StringComparison.OrdinalIgnoreCase);
                }
                else // "unity" default
                {
                    include = asmName.StartsWith("UnityEngine", StringComparison.OrdinalIgnoreCase) ||
                              asmName.StartsWith("UnityEditor", StringComparison.OrdinalIgnoreCase) ||
                              asmName.StartsWith("Unity.", StringComparison.OrdinalIgnoreCase);
                }

                if (!include)
                    continue;

                try
                {
                    Type[] exported = asm.GetExportedTypes();
                    types.AddRange(exported);
                }
                catch (ReflectionTypeLoadException ex)
                {
                    if (ex.Types != null)
                    {
                        for (int t = 0; t < ex.Types.Length; t++)
                        {
                            if (ex.Types[t] != null)
                                types.Add(ex.Types[t]);
                        }
                    }
                }
                catch
                {
                    // Skip inaccessible dynamic assemblies
                }
            }

            return types;
        }

        private static Type ResolveType(string typeName)
        {
            if (string.IsNullOrWhiteSpace(typeName))
                return null;

            string name = typeName.Trim();

            // Direct Type.GetType lookup
            Type direct = Type.GetType(name, false, true);
            if (direct != null)
                return direct;

            // Search loaded types
            List<Type> types = GetLoadedTypes("all");

            // Exact FullName match
            for (int i = 0; i < types.Count; i++)
            {
                if (string.Equals(types[i].FullName, name, StringComparison.OrdinalIgnoreCase))
                    return types[i];
            }

            // Exact Name match
            for (int i = 0; i < types.Count; i++)
            {
                if (string.Equals(types[i].Name, name, StringComparison.OrdinalIgnoreCase))
                    return types[i];
            }

            return null;
        }

        private static string FormatTypeName(Type type)
        {
            if (type == null)
                return "void";

            if (s_FriendlyTypeNames.TryGetValue(type, out string friendly))
                return friendly;

            if (type.IsArray)
                return FormatTypeName(type.GetElementType()) + "[]";

            if (type.IsGenericType)
            {
                string genDef = type.Name;
                int tickIdx = genDef.IndexOf('`');
                if (tickIdx > 0)
                    genDef = genDef.Substring(0, tickIdx);

                Type[] args = type.GetGenericArguments();
                StringBuilder sb = new StringBuilder();
                sb.Append(genDef).Append("<");
                for (int i = 0; i < args.Length; i++)
                {
                    if (i > 0)
                        sb.Append(", ");
                    sb.Append(FormatTypeName(args[i]));
                }
                sb.Append(">");
                return sb.ToString();
            }

            return type.Name;
        }

        #endregion

        #region 3. Asset SHA Checksum & Verification (get_sha)

        [CliCommand("get_sha", "Compute SHA256 checksum of an asset or file on disk (IvanMurzak get_sha compatibility)", MainThreadRequired = false, Tags = new[] { "assets", "compat" })]
        public static object GetSha(
            [CliArg("path", "Relative asset path or absolute file path to inspect")] string path)
        {
            if (string.IsNullOrWhiteSpace(path))
                return new { success = false, message = "'path' parameter is required." };

            string fullPath = ResolveFilePath(path);
            if (!File.Exists(fullPath))
                return new { success = false, message = "File does not exist: " + fullPath };

            try
            {
                using (SHA256 sha = SHA256.Create())
                using (FileStream stream = File.OpenRead(fullPath))
                {
                    byte[] hash = sha.ComputeHash(stream);
                    StringBuilder sb = new StringBuilder();
                    for (int i = 0; i < hash.Length; i++)
                        sb.Append(hash[i].ToString("x2"));

                    FileInfo info = new FileInfo(fullPath);
                    return new
                    {
                        success = true,
                        path = path,
                        full_path = fullPath,
                        sha256 = sb.ToString(),
                        size_bytes = info.Length,
                        last_modified = info.LastWriteTimeUtc.ToString("o")
                    };
                }
            }
            catch (Exception ex)
            {
                return new { success = false, message = "Error computing SHA256: " + ex.Message };
            }
        }

        #endregion

        #region 4. Find in Project Files (find_in_file)

        [CliCommand("find_in_file", "Search project files for text or regex patterns with line numbers (IvanMurzak find_in_file compatibility)", MainThreadRequired = false, Tags = new[] { "assets", "search", "compat" })]
        public static object FindInFile(
            [CliArg("pattern", "Text substring or regex pattern to search for")] string pattern,
            [CliArg("project_root", "Subdirectory to restrict search to (default 'Assets')")] string project_root = "Assets",
            [CliArg("ignore_case", "Case insensitive matching (default true)")] bool ignore_case = true,
            [CliArg("max_results", "Maximum matches to return (default 100)")] int max_results = 100)
        {
            if (string.IsNullOrWhiteSpace(pattern))
                return new { success = false, message = "'pattern' parameter is required." };

            if (max_results <= 0)
                max_results = 100;

            string rootDir = ResolveFilePath(string.IsNullOrWhiteSpace(project_root) ? "Assets" : project_root);
            if (!Directory.Exists(rootDir))
                return new { success = false, message = "Directory does not exist: " + rootDir };

            Regex regex;
            try
            {
                RegexOptions opts = RegexOptions.Compiled;
                if (ignore_case)
                    opts |= RegexOptions.IgnoreCase;
                regex = new Regex(pattern, opts);
            }
            catch (Exception ex)
            {
                return new { success = false, message = "Invalid regex pattern: " + ex.Message };
            }

            string[] extensions = new[] { ".cs", ".shader", ".hlsl", ".cginc", ".uxml", ".uss", ".json", ".txt", ".md", ".yaml" };
            List<object> matches = new List<object>();

            try
            {
                string[] files = Directory.GetFiles(rootDir, "*.*", SearchOption.AllDirectories);
                for (int i = 0; i < files.Length; i++)
                {
                    string f = files[i];
                    string ext = Path.GetExtension(f).ToLowerInvariant();
                    if (!extensions.Contains(ext))
                        continue;

                    string[] lines;
                    try
                    {
                        lines = File.ReadAllLines(f);
                    }
                    catch
                    {
                        continue; // Skip files locked or unreadable
                    }

                    for (int l = 0; l < lines.Length; l++)
                    {
                        if (regex.IsMatch(lines[l]))
                        {
                            string relPath = f.Replace('\\', '/');
                            int assetsIdx = relPath.IndexOf("Assets/", StringComparison.OrdinalIgnoreCase);
                            if (assetsIdx >= 0)
                                relPath = relPath.Substring(assetsIdx);

                            matches.Add(new
                            {
                                file = relPath,
                                line = l + 1,
                                text = lines[l].Trim()
                            });

                            if (matches.Count >= max_results)
                                break;
                        }
                    }

                    if (matches.Count >= max_results)
                        break;
                }
            }
            catch (Exception ex)
            {
                return new { success = false, message = "File search error: " + ex.Message };
            }

            return new
            {
                success = true,
                pattern = pattern,
                count = matches.Count,
                matches = matches
            };
        }

        #endregion

        #region 5. Targeted Text Editing (apply_text_edits)

        [CliCommand("apply_text_edits", "Apply targeted text replacements to a file with optional SHA256 precondition verification (IvanMurzak apply_text_edits compatibility)", MainThreadRequired = true, Tags = new[] { "assets", "text", "compat" })]
        public static object ApplyTextEdits(
            [CliArg("path", "File path to edit relative to project or Assets/")] string path,
            [CliArg("old_text", "Existing text to find and replace")] string old_text = "",
            [CliArg("new_text", "Replacement text")] string new_text = "",
            [CliArg("precondition_sha256", "Optional SHA256 hash that the file must match prior to editing")] string precondition_sha256 = "")
        {
            if (string.IsNullOrWhiteSpace(path))
                return new { success = false, message = "'path' parameter is required." };

            if (old_text == null)
                return new { success = false, message = "'old_text' parameter cannot be null." };

            string fullPath = ResolveFilePath(path);
            if (!File.Exists(fullPath))
                return new { success = false, message = "File does not exist: " + fullPath };

            string content;
            try
            {
                content = File.ReadAllText(fullPath, Encoding.UTF8);
            }
            catch (Exception ex)
            {
                return new { success = false, message = "Failed to read file: " + ex.Message };
            }

            // Precondition verification
            if (!string.IsNullOrWhiteSpace(precondition_sha256))
            {
                string curSha;
                using (SHA256 sha = SHA256.Create())
                {
                    byte[] bytes = sha.ComputeHash(Encoding.UTF8.GetBytes(content));
                    StringBuilder sb = new StringBuilder();
                    for (int i = 0; i < bytes.Length; i++)
                        sb.Append(bytes[i].ToString("x2"));
                    curSha = sb.ToString();
                }

                if (!string.Equals(curSha, precondition_sha256.Trim(), StringComparison.OrdinalIgnoreCase))
                {
                    return new
                    {
                        success = false,
                        message = "Precondition failed: file SHA256 (" + curSha + ") does not match expected (" + precondition_sha256 + ")."
                    };
                }
            }

            // Check if old_text exists
            if (!content.Contains(old_text))
            {
                return new
                {
                    success = false,
                    message = "Target string 'old_text' was not found in file: " + path
                };
            }

            string updatedContent = content.Replace(old_text, new_text ?? "");
            try
            {
                File.WriteAllText(fullPath, updatedContent, Encoding.UTF8);
                AssetDatabase.ImportAsset(GetAssetPath(fullPath), ImportAssetOptions.ForceUpdate);
            }
            catch (Exception ex)
            {
                return new { success = false, message = "Failed writing edits to file: " + ex.Message };
            }

            return new
            {
                success = true,
                path = path,
                message = "Edits applied successfully to '" + path + "'."
            };
        }

        #endregion

        #region 6. GameObject Duplication & Scene Framing

        [CliCommand("gameobject", "duplicate", "Duplicate a scene GameObject preserving hierarchy, components, and transform (IvanMurzak manage_gameobject duplicate)", MainThreadRequired = true, Tags = new[] { "gameobject", "compat" })]
        public static object DuplicateGameObject(
            [CliArg("target", "Target GameObject name, path, or instance ID")] string target,
            [CliArg("new_name", "Optional name for duplicated GameObject")] string new_name = "")
        {
            if (string.IsNullOrWhiteSpace(target))
                return new { success = false, message = "'target' parameter is required." };

            GameObject original = FindSceneGameObject(target);
            if (original == null)
                return new { success = false, message = "GameObject target '" + target + "' was not found in active scene." };

            GameObject duplicate = UnityEngine.Object.Instantiate(original, original.transform.parent);
            if (!string.IsNullOrWhiteSpace(new_name))
                duplicate.name = new_name.Trim();
            else
                duplicate.name = original.name + " (Duplicate)";

            duplicate.transform.localPosition = original.transform.localPosition;
            duplicate.transform.localRotation = original.transform.localRotation;
            duplicate.transform.localScale = original.transform.localScale;

            Undo.RegisterCreatedObjectUndo(duplicate, "Duplicate GameObject " + duplicate.name);

            return new
            {
                success = true,
                original = original.name,
                duplicate_name = duplicate.name,
                instance_id = duplicate.GetInstanceID()
            };
        }

        [CliCommand("scene", "frame", "Frame SceneView camera on target GameObject or selection (IvanMurzak manage_scene scene_view_frame)", MainThreadRequired = true, Tags = new[] { "scene", "compat" })]
        public static object FrameSceneView(
            [CliArg("target", "Target GameObject name, path, or instance ID to frame in SceneView")] string target = "")
        {
            GameObject go = null;
            if (!string.IsNullOrWhiteSpace(target))
            {
                go = FindSceneGameObject(target);
                if (go == null)
                    return new { success = false, message = "Target GameObject '" + target + "' not found." };
                Selection.activeGameObject = go;
            }
            else
            {
                go = Selection.activeGameObject;
            }

            if (go == null)
                return new { success = false, message = "No target GameObject specified and no active scene selection to frame." };

            SceneView sv = SceneView.lastActiveSceneView;
            if (sv == null && SceneView.sceneViews.Count > 0)
                sv = SceneView.sceneViews[0] as SceneView;

            if (sv == null)
                return new { success = false, message = "No active SceneView available to frame target." };

            sv.FrameSelected();
            sv.Repaint();

            return new
            {
                success = true,
                framed_object = go.name,
                message = "Framed SceneView camera on '" + go.name + "'."
            };
        }

        #endregion

        #region Helpers

        private static string ResolveFilePath(string path)
        {
            if (string.IsNullOrWhiteSpace(path))
                return Application.dataPath;

            string p = path.Trim();
            if (Path.IsPathRooted(p))
                return p;

            string projectDir = Directory.GetParent(Application.dataPath).FullName;
            return Path.Combine(projectDir, p);
        }

        private static string GetAssetPath(string fullPath)
        {
            string norm = fullPath.Replace('\\', '/');
            int idx = norm.IndexOf("Assets/", StringComparison.OrdinalIgnoreCase);
            if (idx >= 0)
                return norm.Substring(idx);
            return fullPath;
        }

        private static GameObject FindSceneGameObject(string target)
        {
            if (string.IsNullOrWhiteSpace(target))
                return null;

            string t = target.Trim();

            // 1. Try instance ID integer
            if (int.TryParse(t, out int id))
            {
                UnityEngine.Object obj = EditorUtility.InstanceIDToObject(id);
                if (obj is GameObject go)
                    return go;
            }

            // 2. Direct name / hierarchy path lookup
            GameObject direct = GameObject.Find(t);
            if (direct != null)
                return direct;

            // 3. Scan all root GameObjects in active scene
            UnityEngine.SceneManagement.Scene activeScene = UnityEngine.SceneManagement.SceneManager.GetActiveScene();
            GameObject[] roots = activeScene.GetRootGameObjects();
            for (int i = 0; i < roots.Length; i++)
            {
                if (string.Equals(roots[i].name, t, StringComparison.OrdinalIgnoreCase))
                    return roots[i];

                Transform[] children = roots[i].GetComponentsInChildren<Transform>(true);
                for (int c = 0; c < children.Length; c++)
                {
                    if (string.Equals(children[c].name, t, StringComparison.OrdinalIgnoreCase))
                        return children[c].gameObject;
                }
            }

            return null;
        }

        #endregion
    }
}
