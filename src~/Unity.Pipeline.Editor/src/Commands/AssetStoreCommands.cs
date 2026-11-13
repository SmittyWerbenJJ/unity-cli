using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Unity.Pipeline.Commands;
using Unity.Pipeline.Editor.Compat;
using UnityEditor;
using UnityEngine;

namespace Unity.Pipeline.Editor.Commands
{
    /// <summary>
    /// Asset Store inspection and download commands exposed directly through the project's
    /// com.unity.pipeline daemon. Requires a Unity Hub-signed-in GUI Editor session
    /// (batch-mode editors do not receive a Hub user session).
    /// Never reads, logs, or stores user access tokens, download URLs, or keys, and never invokes purchases.
    /// </summary>
    public static class AssetStoreCommands
    {
        private const string InternalNamespace = "UnityEditor.PackageManager.UI.Internal.";
        private const BindingFlags AllFlags = BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic;

        [CliCommand("assetstore_status", "Check whether this Unity Editor has an active Unity Hub session for Asset Store commands", MainThreadRequired = true, Tags = new[] { "assetstore", "auth" })]
        public static AssetStoreStatusResponse Status()
        {
            return ReadSessionStatus();
        }

        [CliCommand("assetstore_list", "List owned Asset Store purchases from the signed-in GUI Editor session", MainThreadRequired = true, Tags = new[] { "assetstore", "read" })]
        public static async Task<AssetStoreListResponse> List(
            [CliArg("filter", "Optional case-insensitive regex or substring filter on displayName")] string filter = "",
            [CliArg("offset", "Pagination offset (default 0)")] int offset = 0,
            [CliArg("limit", "Maximum results to fetch per page (default 500)")] int limit = 500,
            [CliArg("wait_seconds", "Seconds to wait for Unity Hub session handoff after GUI launch (default 8)")] int wait_seconds = 8)
        {
            var session = await WaitForSignedInSessionAsync(wait_seconds);
            if (!session.readyForAssetStore)
            {
                return new AssetStoreListResponse
                {
                    success = false,
                    message = session.message
                };
            }

            try
            {
                var raw = await InvokeGetPurchasesAsync(offset, limit);
                var items = ExtractPurchaseItems(raw, filter);
                return new AssetStoreListResponse
                {
                    success = true,
                    total = raw.TryGetValue("total", out object tot) ? Convert.ToInt32(tot) : items.Count,
                    count = items.Count,
                    items = items,
                    message = $"Found {items.Count} matching owned Asset Store packages."
                };
            }
            catch (Exception ex)
            {
                return new AssetStoreListResponse
                {
                    success = false,
                    message = "assetstore_list failed: " + Unwrap(ex).Message
                };
            }
        }

        [CliCommand("assetstore_detail", "Get product metadata and per-Unity-version download sizes for an owned Asset Store package", MainThreadRequired = true, Tags = new[] { "assetstore", "read" })]
        public static async Task<AssetStoreDetailResponse> Detail(
            [CliArg("package_id", "Asset Store numeric packageId")] long package_id = 0,
            [CliArg("wait_seconds", "Seconds to wait for Unity Hub session handoff (default 8)")] int wait_seconds = 8)
        {
            if (package_id <= 0)
            {
                return new AssetStoreDetailResponse { success = false, message = "Required parameter 'package_id' must be > 0." };
            }

            var session = await WaitForSignedInSessionAsync(wait_seconds);
            if (!session.readyForAssetStore)
            {
                return new AssetStoreDetailResponse { success = false, packageId = package_id, message = session.message };
            }

            try
            {
                object rawDetail = await InvokeGetProductDetailAsync(package_id);
                var sizes = new List<AssetStoreSizeInfo>();
                string displayName = string.Empty;
                string publisher = string.Empty;
                string category = string.Empty;

                if (rawDetail is Dictionary<string, object> dict)
                {
                    Type piType = FindInternalType("AssetStoreProductInfo");
                    MethodInfo parseMethod = piType?.GetMethod("ParseProductInfo", AllFlags);
                    if (parseMethod != null)
                    {
                        object utils = ResolveService(FindInternalType("AssetStoreUtils"));
                        object pi = parseMethod.Invoke(null, new object[] { utils, package_id.ToString(), dict });
                        if (pi != null && piType.GetField("sizeInfos", AllFlags)?.GetValue(pi) is IEnumerable sizeList)
                        {
                            ExtractSizeInfos(sizeList, sizes);
                        }
                    }

                    displayName = dict.TryGetValue("name", out object n) ? Convert.ToString(n) : string.Empty;
                    publisher = dict.TryGetValue("publisher", out object p) && p is Dictionary<string, object> pd && pd.TryGetValue("label", out object pl) ? Convert.ToString(pl) : string.Empty;
                    category = dict.TryGetValue("category", out object c) && c is Dictionary<string, object> cd && cd.TryGetValue("label", out object cl) ? Convert.ToString(cl) : string.Empty;
                }
                else if (rawDetail != null)
                {
                    // Unity 6000.0+ returns AssetStoreProductInfo directly
                    displayName = Convert.ToString(ReadMember(rawDetail, "displayName") ?? ReadMember(rawDetail, "packageName") ?? ReadMember(rawDetail, "name")) ?? string.Empty;
                    publisher = Convert.ToString(ReadMember(rawDetail, "publisherName") ?? ReadMember(rawDetail, "publisher")) ?? string.Empty;
                    category = Convert.ToString(ReadMember(rawDetail, "category") ?? ReadMember(rawDetail, "categoryName")) ?? string.Empty;
                    if (ReadMember(rawDetail, "sizeInfos") is IEnumerable sizeList)
                    {
                        ExtractSizeInfos(sizeList, sizes);
                    }
                }

                return new AssetStoreDetailResponse
                {
                    success = true,
                    packageId = package_id,
                    displayName = displayName,
                    publisher = publisher,
                    category = category,
                    sizes = sizes,
                    message = "Fetched product details."
                };
            }
            catch (Exception ex)
            {
                return new AssetStoreDetailResponse
                {
                    success = false,
                    packageId = package_id,
                    message = "assetstore_detail failed: " + Unwrap(ex).Message
                };
            }
        }

        [CliCommand("assetstore_fetch", "Download an owned Asset Store package into the local Unity Asset Store-5.x cache", MainThreadRequired = true, Tags = new[] { "assetstore", "download" })]
        public static async Task<AssetStoreFetchResponse> Fetch(
            [CliArg("package_id", "Numeric Asset Store packageId (optional if name is provided)")] long package_id = 0,
            [CliArg("name", "Package displayName substring/regex to resolve when package_id is omitted")] string name = "",
            [CliArg("timeout_seconds", "Download timeout in seconds (default 120)")] int timeout_seconds = 120,
            [CliArg("wait_seconds", "Seconds to wait for Unity Hub session handoff (default 8)")] int wait_seconds = 8)
        {
            var session = await WaitForSignedInSessionAsync(wait_seconds);
            if (!session.readyForAssetStore)
            {
                return new AssetStoreFetchResponse { success = false, message = session.message };
            }

            try
            {
                if (package_id <= 0)
                {
                    if (string.IsNullOrWhiteSpace(name))
                    {
                        return new AssetStoreFetchResponse
                        {
                            success = false,
                            message = "Provide either 'package_id' or 'name'."
                        };
                    }

                    var raw = await InvokeGetPurchasesAsync(0, 500);
                    var matches = ExtractPurchaseItems(raw, name);
                    if (matches.Count == 0)
                    {
                        return new AssetStoreFetchResponse
                        {
                            success = false,
                            message = $"No owned Asset Store package matched '{name}'."
                        };
                    }

                    var exact = matches.FirstOrDefault(m => string.Equals(m.displayName, name.Trim(), StringComparison.OrdinalIgnoreCase));
                    if (exact == null && matches.Count > 1)
                    {
                        string sample = string.Join(", ", matches.Take(5).Select(m => $"{m.packageId}:{m.displayName}"));
                        return new AssetStoreFetchResponse
                        {
                            success = false,
                            message = $"Multiple owned packages matched '{name}' ({sample}). Pass an exact 'package_id'."
                        };
                    }

                    package_id = (exact ?? matches[0]).packageId;
                }

                return await DownloadOwnedPackageAsync(package_id, timeout_seconds);
            }
            catch (Exception ex)
            {
                return new AssetStoreFetchResponse
                {
                    success = false,
                    packageId = package_id,
                    message = "assetstore_fetch failed: " + Unwrap(ex).Message
                };
            }
        }

        #region Session & Version-Specific Adapters

        private static bool IsBatchMode()
        {
            PropertyInfo p = typeof(Application).GetProperty("isBatchMode", BindingFlags.Public | BindingFlags.Static);
            if (p != null && p.GetValue(null) is bool b) return b;
            return Environment.GetCommandLineArgs().Any(a => string.Equals(a, "-batchmode", StringComparison.OrdinalIgnoreCase));
        }

        private static AssetStoreStatusResponse ReadSessionStatus()
        {
            var ver = UnityRuntimeVersion.Current;
            bool batch = IsBatchMode();
            bool loggedIn = false;
            bool online = false;
            bool isUserInfoReady = false;
            bool userInfoValid = false;

            Type ucType = Type.GetType("UnityEditor.Connect.UnityConnect, UnityEditor", false);
            if (ucType != null)
            {
                object inst = ucType.GetProperty("instance", AllFlags)?.GetValue(null);
                if (inst != null)
                {
                    loggedIn = ReadBool(inst, "loggedIn");
                    online = ReadBool(inst, "online");
                    isUserInfoReady = ReadBool(inst, "isUserInfoReady");
                    object userInfo = ucType.GetProperty("userInfo", AllFlags)?.GetValue(inst);
                    if (userInfo != null)
                    {
                        userInfoValid = ReadBool(userInfo, "valid");
                    }
                }
            }

            bool ready = !batch && loggedIn && isUserInfoReady && userInfoValid;
            string msg = ready
                ? "Unity Hub session is active and ready for Asset Store operations."
                : batch
                    ? "Editor is running in -batchmode; Unity Hub does not attach a user session to batchmode editors. Launch as a GUI editor (without -batchmode) to use Asset Store commands."
                    : "Unity Hub session is not ready (loggedIn=" + loggedIn + ", isUserInfoReady=" + isUserInfoReady + ", userInfoValid=" + userInfoValid + "). Ensure Unity Hub is running and signed in.";

            return new AssetStoreStatusResponse
            {
                success = true,
                unityVersion = ver.Raw,
                batchmode = batch,
                loggedIn = loggedIn,
                online = online,
                isUserInfoReady = isUserInfoReady,
                userInfoValid = userInfoValid,
                readyForAssetStore = ready,
                supported = FindInternalType("AssetStoreRestAPI") != null && FindInternalType("AssetStoreDownloadOperation") != null,
                message = msg
            };
        }

        private static async Task<AssetStoreStatusResponse> WaitForSignedInSessionAsync(int waitSeconds)
        {
            var status = ReadSessionStatus();
            if (status.readyForAssetStore || status.batchmode || waitSeconds <= 0)
            {
                return status;
            }

            double deadline = EditorApplication.timeSinceStartup + waitSeconds;
            while (EditorApplication.timeSinceStartup < deadline)
            {
                await NextEditorTickAsync();
                status = ReadSessionStatus();
                if (status.readyForAssetStore)
                {
                    return status;
                }
            }

            return status;
        }

        private static Task<Dictionary<string, object>> InvokeGetPurchasesAsync(int offset, int limit)
        {
            var tcs = new TaskCompletionSource<Dictionary<string, object>>();
            Type apiType = FindInternalType("AssetStoreRestAPI", throwOnMissing: true);
            object api = ResolveService(apiType);

            // Version-specific dispatch:
            // - Unity 2021.3 / 2022.3: GetPurchases(string query, Action<Dictionary<string, object>>, Action<UIError>)
            // - Unity 6000.0+: GetPurchases(PurchasesQueryArgs, Action<PurchasesResult/Dictionary>, Action<UIError>)
            MethodInfo method = apiType.GetMethods(AllFlags).FirstOrDefault(m => m.Name == "GetPurchases" && m.GetParameters().Length == 3);
            if (method == null)
            {
                tcs.SetException(new MissingMethodException("AssetStoreRestAPI.GetPurchases(3 args) not found on Unity " + Application.unityVersion));
                return tcs.Task;
            }

            ParameterInfo[] pars = method.GetParameters();
            object queryArg;
            if (pars[0].ParameterType == typeof(string))
            {
                queryArg = $"?offset={offset}&limit={limit}";
            }
            else
            {
                queryArg = CreatePurchasesQueryArg(pars[0].ParameterType, offset, limit);
            }

            Delegate onDone = CreateCallbackDelegate(pars[1].ParameterType, resultObj =>
            {
                if (resultObj is Dictionary<string, object> dict)
                {
                    tcs.TrySetResult(dict);
                }
                else
                {
                    // Unity 6000+ PurchasesResult object -> normalize into Dictionary<string, object>
                    var normalized = new Dictionary<string, object>
                    {
                        ["total"] = ReadMember(resultObj, "total") ?? 0,
                        ["results"] = ReadMember(resultObj, "results") ?? ReadMember(resultObj, "list") ?? new ArrayList()
                    };
                    tcs.TrySetResult(normalized);
                }
            });

            Delegate onError = CreateErrorDelegate(pars[2].ParameterType, msg => tcs.TrySetException(new InvalidOperationException(msg)));
            method.Invoke(api, new object[] { queryArg, onDone, onError });
            return tcs.Task;
        }

        private static Task<object> InvokeGetProductDetailAsync(long packageId)
        {
            var tcs = new TaskCompletionSource<object>();
            Type apiType = FindInternalType("AssetStoreRestAPI", throwOnMissing: true);
            object api = ResolveService(apiType);

            // Unity 2021.3/2022.3 uses 2 args (long, Action<Dictionary<string, object>>).
            // Unity 6000.0+ uses 3 args (long, Action<AssetStoreProductInfo>, Action<UIError>).
            MethodInfo method3 = apiType.GetMethods(AllFlags).FirstOrDefault(m => m.Name == "GetProductDetail" && m.GetParameters().Length == 3);
            if (method3 != null)
            {
                ParameterInfo[] pars = method3.GetParameters();
                Delegate ok = CreateCallbackDelegate(pars[1].ParameterType, d => tcs.TrySetResult(d));
                Delegate err = CreateErrorDelegate(pars[2].ParameterType, msg => tcs.TrySetException(new InvalidOperationException(msg)));
                method3.Invoke(api, new object[] { packageId, ok, err });
                return tcs.Task;
            }

            MethodInfo method2 = apiType.GetMethods(AllFlags).FirstOrDefault(m => m.Name == "GetProductDetail" && m.GetParameters().Length == 2);
            if (method2 == null)
            {
                tcs.SetException(new MissingMethodException("AssetStoreRestAPI.GetProductDetail not found on Unity " + Application.unityVersion));
                return tcs.Task;
            }

            ParameterInfo[] pars2 = method2.GetParameters();
            Delegate ok2 = CreateCallbackDelegate(pars2[1].ParameterType, d => tcs.TrySetResult(d));
            method2.Invoke(api, new object[] { packageId, ok2 });
            return tcs.Task;
        }

        private static object CreatePurchasesQueryArg(Type queryType, int offset, int limit)
        {
            ConstructorInfo defaultCtor = queryType.GetConstructor(AllFlags, null, Type.EmptyTypes, null);
            if (defaultCtor != null)
            {
                object inst = defaultCtor.Invoke(null);
                SetMemberIfExists(inst, "startIndex", offset);
                SetMemberIfExists(inst, "limit", limit);
                return inst;
            }

            // Unity 6000.3+: PurchasesQueryArgs(int startIndex, int limit, string searchText, PageFilters filters)
            ConstructorInfo ctor = queryType
                .GetConstructors(AllFlags)
                .OrderBy(c => c.GetParameters().Length)
                .FirstOrDefault(c => c.GetParameters().Length >= 2
                    && c.GetParameters()[0].ParameterType == typeof(int)
                    && c.GetParameters()[1].ParameterType == typeof(int));

            if (ctor == null)
            {
                throw new MissingMethodException("No compatible PurchasesQueryArgs constructor found on Unity " + Application.unityVersion);
            }

            ParameterInfo[] cp = ctor.GetParameters();
            object[] args = new object[cp.Length];
            args[0] = offset;
            args[1] = limit;
            for (int i = 2; i < cp.Length; i++)
            {
                args[i] = cp[i].HasDefaultValue ? cp[i].DefaultValue : null;
            }
            return ctor.Invoke(args);
        }

        private static void ExtractSizeInfos(IEnumerable sizeList, List<AssetStoreSizeInfo> target)
        {
            foreach (object si in sizeList)
            {
                target.Add(new AssetStoreSizeInfo
                {
                    supportedUnityVersion = Convert.ToString(ReadMember(si, "supportedUnityVersion")),
                    downloadSize = Convert.ToInt64(ReadMember(si, "downloadSize") ?? 0L),
                    assetCount = Convert.ToInt32(ReadMember(si, "assetCount") ?? 0)
                });
            }
        }

        private static async Task<AssetStoreFetchResponse> DownloadOwnedPackageAsync(long packageId, int timeoutSeconds)
        {
            Type mgrType = FindInternalType("AssetStoreDownloadManager", throwOnMissing: true);
            Type opType = FindInternalType("AssetStoreDownloadOperation", throwOnMissing: true);
            object mgr = ResolveService(mgrType);

            mgrType.GetMethod("RegisterDownloadDelegate", AllFlags)?.Invoke(mgr, null);

            object op = ConstructDownloadOperation(opType, packageId);
            mgrType.GetMethod("SetupDownloadOperation", AllFlags)?.Invoke(mgr, new object[] { op });

            MethodInfo downloadMethod = opType.GetMethod("Download", AllFlags);
            if (downloadMethod == null)
            {
                throw new MissingMethodException("AssetStoreDownloadOperation.Download not found.");
            }
            downloadMethod.Invoke(op, new object[] { false });

            double start = EditorApplication.timeSinceStartup;
            double deadline = start + Math.Max(10, timeoutSeconds);
            bool sawProgress = false;

            while (EditorApplication.timeSinceStartup < deadline)
            {
                await NextEditorTickAsync();
                bool inProgress = ReadBool(op, "isInProgress");
                object info = ReadMember(op, "downloadInfo");
                if (inProgress)
                {
                    sawProgress = true;
                }
                else if (info != null && (sawProgress || EditorApplication.timeSinceStartup - start > 1.0))
                {
                    break;
                }
            }

            string err = Convert.ToString(ReadMember(op, "errorMessage"));
            string newPath = Convert.ToString(ReadMember(op, "packageNewPath"));
            object finalInfo = ReadMember(op, "downloadInfo");
            string publisher = finalInfo != null ? Convert.ToString(ReadMember(finalInfo, "publisherName")) : string.Empty;
            string category = finalInfo != null ? Convert.ToString(ReadMember(finalInfo, "categoryName")) : string.Empty;
            string packageName = finalInfo != null ? Convert.ToString(ReadMember(finalInfo, "packageName")) : string.Empty;

            if (!string.IsNullOrEmpty(err))
            {
                return new AssetStoreFetchResponse
                {
                    success = false,
                    packageId = packageId,
                    publisher = publisher,
                    category = category,
                    packageName = packageName,
                    message = "Download failed: " + err
                };
            }

            if (string.IsNullOrEmpty(newPath) || !File.Exists(newPath))
            {
                return new AssetStoreFetchResponse
                {
                    success = false,
                    packageId = packageId,
                    publisher = publisher,
                    category = category,
                    packageName = packageName,
                    message = "Download did not produce a local .unitypackage within timeout."
                };
            }

            long bytes = new FileInfo(newPath).Length;
            return new AssetStoreFetchResponse
            {
                success = true,
                packageId = packageId,
                packagePath = newPath,
                bytes = bytes,
                publisher = publisher,
                category = category,
                packageName = packageName,
                message = $"Downloaded {packageName} ({bytes} bytes) to {newPath}."
            };
        }

        /// <summary>
        /// Version-agnostic constructor binder for AssetStoreDownloadOperation across:
        /// - 2021.3: (AssetStoreUtils, AssetStoreRestAPI, string, string)
        /// - 2022.3: (AssetStoreUtils, AssetStoreRestAPI, AssetStoreCachePathProxy, string, string)
        /// - 6000.3+: (IIOProxy, IAssetStoreUtils, IAssetStoreRestAPI, IAssetStoreCache, IAssetStoreCachePathProxy, ILocalInfoHandler, long, string)
        /// </summary>
        private static object ConstructDownloadOperation(Type opType, long packageId)
        {
            ConstructorInfo bestCtor = opType
                .GetConstructors(AllFlags)
                .OrderByDescending(c => c.GetParameters().Length)
                .FirstOrDefault(c => c.GetParameters().Length >= 4);

            if (bestCtor == null)
            {
                throw new MissingMethodException("No compatible AssetStoreDownloadOperation constructor found on Unity " + Application.unityVersion);
            }

            ParameterInfo[] pars = bestCtor.GetParameters();
            object[] args = new object[pars.Length];
            bool assignedProductId = false;

            for (int i = 0; i < pars.Length; i++)
            {
                Type pt = pars[i].ParameterType;
                if (!assignedProductId && (pt == typeof(long) || pt == typeof(int)))
                {
                    args[i] = Convert.ChangeType(packageId, pt);
                    assignedProductId = true;
                }
                else if (!assignedProductId && pt == typeof(string))
                {
                    args[i] = packageId.ToString();
                    assignedProductId = true;
                }
                else if (pt == typeof(string))
                {
                    args[i] = null;
                }
                else
                {
                    args[i] = ResolveService(pt);
                }
            }

            return bestCtor.Invoke(args);
        }

        #endregion

        #region Reflection & Extraction Helpers

        private static List<AssetStorePurchaseItem> ExtractPurchaseItems(Dictionary<string, object> raw, string filter)
        {
            var items = new List<AssetStorePurchaseItem>();
            if (!raw.TryGetValue("results", out object resultsObj) || !(resultsObj is IEnumerable list))
            {
                return items;
            }

            Regex rx = null;
            if (!string.IsNullOrWhiteSpace(filter))
            {
                try { rx = new Regex(filter.Trim(), RegexOptions.IgnoreCase); }
                catch { rx = null; }
            }

            foreach (object entry in list)
            {
                if (entry == null) continue;
                long pid;
                string displayName;
                bool hidden;

                if (entry is Dictionary<string, object> d)
                {
                    object pObj = null;
                    if (!d.TryGetValue("packageId", out pObj) && !d.TryGetValue("productId", out pObj))
                    {
                        d.TryGetValue("id", out pObj);
                    }
                    pid = pObj != null ? Convert.ToInt64(pObj) : 0L;
                    displayName = d.TryGetValue("displayName", out object nObj) ? Convert.ToString(nObj) : string.Empty;
                    hidden = d.TryGetValue("isHidden", out object hObj) && Convert.ToBoolean(hObj);
                }
                else
                {
                    pid = Convert.ToInt64(ReadMember(entry, "packageId") ?? ReadMember(entry, "productId") ?? ReadMember(entry, "id") ?? 0L);
                    displayName = Convert.ToString(ReadMember(entry, "displayName")) ?? string.Empty;
                    hidden = Convert.ToBoolean(ReadMember(entry, "isHidden") ?? false);
                }

                if (!string.IsNullOrWhiteSpace(filter))
                {
                    bool matched = rx != null
                        ? rx.IsMatch(displayName)
                        : displayName.IndexOf(filter.Trim(), StringComparison.OrdinalIgnoreCase) >= 0;
                    if (!matched) continue;
                }

                items.Add(new AssetStorePurchaseItem
                {
                    packageId = pid,
                    displayName = displayName,
                    isHidden = hidden
                });
            }

            return items;
        }

        private static Type FindInternalType(string simpleName, bool throwOnMissing = false)
        {
            string fullName = InternalNamespace + simpleName;
            Type t = typeof(UnityEditor.Editor).Assembly.GetType(fullName, false);
            if (t != null) return t;

            foreach (Assembly asm in AppDomain.CurrentDomain.GetAssemblies())
            {
                if (!asm.GetName().Name.StartsWith("UnityEditor", StringComparison.Ordinal)) continue;
                t = asm.GetType(fullName, false);
                if (t != null) return t;
            }

            if (throwOnMissing)
            {
                throw new TypeLoadException($"Internal Unity type '{fullName}' was not found on Unity {Application.unityVersion}.");
            }
            return null;
        }

        private static object ResolveService(Type serviceType)
        {
            if (serviceType == null) return null;
            Type scType = FindInternalType("ServicesContainer", throwOnMissing: true);
            object scInst = scType.BaseType?.GetProperty("instance", AllFlags)?.GetValue(null);
            MethodInfo resolveMethod = scType.GetMethod("Resolve", AllFlags)?.MakeGenericMethod(serviceType);
            return resolveMethod?.Invoke(scInst, null);
        }

        private static Delegate CreateCallbackDelegate(Type delegateType, Action<object> handler)
        {
            Type argType = delegateType.GetGenericArguments()[0];
            var box = new CallbackBox(handler);
            MethodInfo bridge = typeof(CallbackBox).GetMethod(nameof(CallbackBox.OnValue), BindingFlags.Public | BindingFlags.Instance)?.MakeGenericMethod(argType);
            return Delegate.CreateDelegate(delegateType, box, bridge);
        }

        private static Delegate CreateErrorDelegate(Type delegateType, Action<string> onError)
        {
            Type errType = delegateType.GetGenericArguments()[0];
            var box = new CallbackBox(errObj =>
            {
                string msg = Convert.ToString(ReadMember(errObj, "message")) ?? Convert.ToString(errObj) ?? "Unknown Asset Store error";
                onError(msg);
            });
            MethodInfo bridge = typeof(CallbackBox).GetMethod(nameof(CallbackBox.OnValue), BindingFlags.Public | BindingFlags.Instance)?.MakeGenericMethod(errType);
            return Delegate.CreateDelegate(delegateType, box, bridge);
        }

        private sealed class CallbackBox
        {
            private readonly Action<object> _handler;
            public CallbackBox(Action<object> handler) => _handler = handler;
            public void OnValue<T>(T value) => _handler(value);
        }

        private static Task NextEditorTickAsync()
        {
            var tcs = new TaskCompletionSource<bool>();
            EditorApplication.CallbackFunction once = null;
            once = () =>
            {
                EditorApplication.update -= once;
                tcs.TrySetResult(true);
            };
            EditorApplication.update += once;
            return tcs.Task;
        }

        private static object ReadMember(object obj, string name)
        {
            if (obj == null) return null;
            Type t = obj.GetType();
            PropertyInfo p = t.GetProperty(name, AllFlags);
            if (p != null && p.GetIndexParameters().Length == 0) return p.GetValue(obj);
            FieldInfo f = t.GetField(name, AllFlags);
            return f?.GetValue(obj);
        }

        private static bool ReadBool(object obj, string name)
        {
            object v = ReadMember(obj, name);
            return v is bool b && b;
        }

        private static void SetMemberIfExists(object obj, string name, object value)
        {
            if (obj == null) return;
            Type t = obj.GetType();
            PropertyInfo p = t.GetProperty(name, AllFlags);
            if (p != null && p.CanWrite) { p.SetValue(obj, value); return; }
            FieldInfo f = t.GetField(name, AllFlags);
            if (f != null && !f.IsInitOnly) f.SetValue(obj, value);
        }

        private static Exception Unwrap(Exception ex) => ex is TargetInvocationException tie && tie.InnerException != null ? tie.InnerException : ex;

        #endregion

        #region Response DTOs

        public class AssetStoreStatusResponse
        {
            public bool success;
            public string unityVersion;
            public bool batchmode;
            public bool loggedIn;
            public bool online;
            public bool isUserInfoReady;
            public bool userInfoValid;
            public bool readyForAssetStore;
            public bool supported;
            public string message;
        }

        public class AssetStorePurchaseItem
        {
            public long packageId;
            public string displayName;
            public bool isHidden;
        }

        public class AssetStoreListResponse
        {
            public bool success;
            public int total;
            public int count;
            public List<AssetStorePurchaseItem> items = new List<AssetStorePurchaseItem>();
            public string message;
        }

        public class AssetStoreSizeInfo
        {
            public string supportedUnityVersion;
            public long downloadSize;
            public int assetCount;
        }

        public class AssetStoreDetailResponse
        {
            public bool success;
            public long packageId;
            public string displayName;
            public string publisher;
            public string category;
            public List<AssetStoreSizeInfo> sizes = new List<AssetStoreSizeInfo>();
            public string message;
        }

        public class AssetStoreFetchResponse
        {
            public bool success;
            public long packageId;
            public string packagePath;
            public long bytes;
            public string publisher;
            public string category;
            public string packageName;
            public string message;
        }

        #endregion
    }
}
