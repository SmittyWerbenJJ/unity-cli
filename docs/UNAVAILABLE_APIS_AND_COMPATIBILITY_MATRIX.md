# Unity Pipeline: Master Command Compatibility Matrix

> [!NOTE]
> **Data Source:** Generated from external dataset [`compatibility_data.json`](./compatibility_data.json) via `generate_matrix.py`.
> Total commands inventoried: **177**. Sorted by **most incompatible first**.

## Master Compatibility Table

| apiCall | Unity 2018.1 | Unity 2019.4 | Unity 2022.3 | Unity 6000.0+ |
| :--- | :---: | :---: | :---: | :---: |
| **`build create_profile`**<br/>`object CreateProfile(string name = "", string platform = "", string subtarget = "", string folder = "", bool activate = false, bool confirm = false, bool dryRun = false) ` | ⬜ | ⬜ | ⬜ | ✅ |
| **`build list_platforms`**<br/>`object ListPlatforms(bool showAll = false) ` | ⬜ | ⬜ | ⬜ | ✅ |
| **`build list_profiles`**<br/>`object ListProfiles() ` | ⬜ | ⬜ | ⬜ | ✅ |
| **`build set_profile`**<br/>`object SetProfile(string name = "", string folder = "", bool? overrideGlobalScenes = null, string[] scenes = null, string[] scriptingDefines = null, bool confirm = false, bool dryRun = false) ` | ⬜ | ⬜ | ⬜ | ✅ |
| **`capture editor_element`**<br/>`CaptureElementResponse EditorElement(string window = "", string selector = "", string output = "") ` | ⬜ | ⬜ | ⬜ | ✅ |
| **`capture runtime_element`**<br/>`CaptureElementResponse RuntimeElement(string panel = "", string selector = "", string output = "") ` | ⬜ | ⬜ | ⬜ | ✅ |
| **`editor.playmode create_scenario`**<br/>`object CreateScenario(string name, string path = null) ` | ⬜ | ⬜ | ⬜ | ✅ |
| **`editor.playmode get_scenario_status`**<br/>`object GetScenarioStatus() ` | ⬜ | ⬜ | ⬜ | ✅ |
| **`editor.playmode list_scenarios`**<br/>`object ListScenarios() ` | ⬜ | ⬜ | ⬜ | ✅ |
| **`editor.playmode set_active_scenario`**<br/>`object SetActiveScenario(string path) ` | ⬜ | ⬜ | ⬜ | ✅ |
| **`editor.playmode start_scenario`**<br/>`object StartScenario() ` | ⬜ | ⬜ | ⬜ | ✅ |
| **`editor.playmode stop_scenario`**<br/>`object StopScenario() ` | ⬜ | ⬜ | ⬜ | ✅ |
| **`multiplayer.role get_active`**<br/>`object GetActive() ` | ⬜ | ⬜ | ⬜ | ✅ |
| **`multiplayer.role get_build`**<br/>`object GetBuild(string profile = null) ` | ⬜ | ⬜ | ⬜ | ✅ |
| **`multiplayer.role get_component`**<br/>`object GetComponent(ObjectRef target, string type = null) ` | ⬜ | ⬜ | ⬜ | ✅ |
| **`multiplayer.role get_game_object`**<br/>`object GetGameObject(ObjectRef target) ` | ⬜ | ⬜ | ⬜ | ✅ |
| **`multiplayer.role list_component_types`**<br/>`object ListComponentTypes() ` | ⬜ | ⬜ | ⬜ | ✅ |
| **`multiplayer.role set_active`**<br/>`object SetActive(string role) ` | ⬜ | ⬜ | ⬜ | ✅ |
| **`multiplayer.role set_build`**<br/>`object SetBuild(string role, string profile = null) ` | ⬜ | ⬜ | ⬜ | ✅ |
| **`multiplayer.role set_component`**<br/>`object SetComponent(ObjectRef target, string role, string type = null) ` | ⬜ | ⬜ | ⬜ | ✅ |
| **`multiplayer.role set_component_type`**<br/>`object SetComponentType(string type, string role) ` | ⬜ | ⬜ | ⬜ | ✅ |
| **`multiplayer.role set_game_object`**<br/>`object SetGameObject(ObjectRef target, string role) ` | ⬜ | ⬜ | ⬜ | ✅ |
| **`diagnostics audit_status`**<br/>`JToken AuditStatus() ` | ⬜ | ⬜ | ✅ | ✅ |
| **`diagnostics memory_snapshot`**<br/>`object MemorySnapshot(string flags = "", string output = "", bool collectGarbage = true, double timeoutSeconds = DefaultTimeoutSeconds) ` | ⬜ | ⬜ | ✅ | ✅ |
| **`diagnostics memory_snapshot_status`**<br/>`JToken MemorySnapshotStatus() ` | ⬜ | ⬜ | ✅ | ✅ |
| **`diagnostics profiler_record`**<br/>`object ProfilerRecord(int frames = DefaultFrames, string output = "", bool deepProfile = false, double timeoutSeconds = DefaultTimeoutSeconds) ` | ⬜ | ⬜ | ✅ | ✅ |
| **`diagnostics profiler_record_status`**<br/>`JToken ProfilerRecordStatus() ` | ⬜ | ⬜ | ✅ | ✅ |
| **`diagnostics profiler_record_stop`**<br/>`object ProfilerRecordStop() ` | ⬜ | ⬜ | ✅ | ✅ |
| **`diagnostics run_audit`**<br/>`object RunAudit(string categories = "", string output = "") ` | ⬜ | ⬜ | ✅ | ✅ |
| **`animation.timeline add_clip`**<br/>`object AddClip(ObjectRef timeline, string track = null, float start = 0f, float duration = 0f, ObjectRef asset = null, bool dryRun = false) ` | ⬜ | ✅ | ✅ | ✅ |
| **`animation.timeline add_track`**<br/>`object AddTrack(ObjectRef timeline, string trackType = null, string name = null, string parentTrack = null, bool dryRun = false) ` | ⬜ | ✅ | ✅ | ✅ |
| **`animation.timeline create`**<br/>`object Create(string path, float frameRate = 60f, bool confirm = false, bool dryRun = false) ` | ⬜ | ✅ | ✅ | ✅ |
| **`animation.timeline get`**<br/>`object Get(ObjectRef timeline) ` | ⬜ | ✅ | ✅ | ✅ |
| **`capture editor_window`**<br/>`CaptureResult EditorWindow(string window = null, string savePath = null, bool includeInlineImage = false, int maxResolution = 0) ` | ⬜ | ✅ | ✅ | ✅ |
| **`capture game_view`**<br/>`CaptureResult GameView(int width = 1280, int height = 720, string camera = null, string savePath = null, bool includeInlineImage = false, int maxResolution = 0, string source = null, string overlay = null) ` | ⬜ | ✅ | ✅ | ✅ |
| **`capture scene_view`**<br/>`CaptureResult SceneView(int width = DefaultWidth, int height = DefaultHeight, string savePath = null, bool includeInlineImage = false, int maxResolution = 0, string source = null) ` | ⬜ | ✅ | ✅ | ✅ |
| **`navmesh bake_surfaces`**<br/>`object BakeSurfaces() ` | ⬜ | ✅ | ✅ | ✅ |
| **`prefab apply_overrides`**<br/>`AuthoringResult ApplyOverrides(ObjectRef instance) ` | ⬜ | ✅ | ✅ | ✅ |
| **`prefab create_variant`**<br/>`AuthoringResult CreateVariant(ObjectRef basePrefab, string path) ` | ⬜ | ✅ | ✅ | ✅ |
| **`prefab save_contents`**<br/>`AuthoringResult SaveContents(ObjectRef prefab, string renameChild = null, string newName = null, string setActiveChild = null, bool active = true) ` | ⬜ | ✅ | ✅ | ✅ |
| **`animation.animator add_layer`**<br/>`object AddLayer(ObjectRef controller, string name, float weight = 1f, string blendingMode = "Override", bool dryRun = false) ` | ✅ | ✅ | ✅ | ✅ |
| **`animation.animator add_parameter`**<br/>`object AddParameter(ObjectRef controller, string name, string type = null, JToken defaultValue = null, bool dryRun = false) ` | ✅ | ✅ | ✅ | ✅ |
| **`animation.animator add_state`**<br/>`object AddState(ObjectRef controller, JToken layer = null, string name = null, ObjectRef motion = null, bool isDefault = false, node position in the graph (cosmetic).")] JArray position = null, bool dryRun = false) ` | ✅ | ✅ | ✅ | ✅ |
| **`animation.animator add_transition`**<br/>`object AddTransition(ObjectRef controller, JToken layer = null, string fromState = null, string toState = null,) ` | ✅ | ✅ | ✅ | ✅ |
| **`animation.animator create_controller`**<br/>`AuthoringResult CreateController(string path, bool confirm = false, bool dryRun = false) ` | ✅ | ✅ | ✅ | ✅ |
| **`animation.animator get_controller`**<br/>`AnimatorControllerInfo GetController(ObjectRef controller) ` | ✅ | ✅ | ✅ | ✅ |
| **`animation.clip create`**<br/>`AuthoringResult Create(string path, float frameRate = 60f, bool loop = false, bool confirm = false, bool dryRun = false) ` | ✅ | ✅ | ✅ | ✅ |
| **`animation.clip get`**<br/>`AnimationClipInfo Get(ObjectRef clip, bool includeKeys = false) ` | ✅ | ✅ | ✅ | ✅ |
| **`animation.clip remove_curve`**<br/>`SetAnimationCurveResult RemoveCurve(ObjectRef clip, string path = "", string type = null, string property = null, bool confirm = false, bool dryRun = false) ` | ✅ | ✅ | ✅ | ✅ |
| **`animation.clip set_curve`**<br/>`SetAnimationCurveResult SetCurve(ObjectRef clip, string path = "", string type = null, string property = null,) ` | ✅ | ✅ | ✅ | ✅ |
| **`asset copy`**<br/>`AuthoringResult Copy(ObjectRef asset, string destination, bool confirm = false, bool dryRun = false) ` | ✅ | ✅ | ✅ | ✅ |
| **`asset create`**<br/>`AuthoringResult Create(string path, string type, string shader = null, bool confirm = false, bool dryRun = false) ` | ✅ | ✅ | ✅ | ✅ |
| **`asset create_folder`**<br/>`AuthoringResult CreateFolder(string path) ` | ✅ | ✅ | ✅ | ✅ |
| **`asset delete`**<br/>`AuthoringResult Delete(ObjectRef asset, bool confirm = false, bool dryRun = false) ` | ✅ | ✅ | ✅ | ✅ |
| **`asset find`**<br/>`FindAssetsResult Find(string type = null, string name = null, string label = null, string searchIn = null, int limit = 200) ` | ✅ | ✅ | ✅ | ✅ |
| **`asset get_import_settings`**<br/>`GetImportSettingsResult GetImportSettings(ObjectRef asset, string platform = "Default") ` | ✅ | ✅ | ✅ | ✅ |
| **`asset import`**<br/>`AuthoringResult Import(string source, string path, bool confirm = false, bool dryRun = false) ` | ✅ | ✅ | ✅ | ✅ |
| **`asset move`**<br/>`AuthoringResult Move(ObjectRef asset, string destination, bool dryRun = false) ` | ✅ | ✅ | ✅ | ✅ |
| **`asset read_text`**<br/>`ReadTextFileResult ReadText(string path, int maxBytes = 1024 * 1024) ` | ✅ | ✅ | ✅ | ✅ |
| **`asset rename`**<br/>`AuthoringResult Rename(ObjectRef asset, string newName, bool dryRun = false) ` | ✅ | ✅ | ✅ | ✅ |
| **`asset set_import_settings`**<br/>`SetImportSettingsResult SetImportSettings(ObjectRef asset,) ` | ✅ | ✅ | ✅ | ✅ |
| **`asset write_text`**<br/>`AuthoringResult WriteText(string path, string contents, bool confirm = false, bool dryRun = false) ` | ✅ | ✅ | ✅ | ✅ |
| **`authoring get_root`**<br/>`object GetRoot() ` | ✅ | ✅ | ✅ | ✅ |
| **`authoring set_root`**<br/>`object SetRoot(string root) ` | ✅ | ✅ | ✅ | ✅ |
| **`build get_settings`**<br/>`object GetSettings() ` | ✅ | ✅ | ✅ | ✅ |
| **`build list`**<br/>`object List() ` | ✅ | ✅ | ✅ | ✅ |
| **`build run`**<br/>`object Run(string target = "", string outputPath = "", string profileName = "", string[] options = null, string[] scenes = null, bool confirm = false, bool dryRun = false, double timeoutInSeconds = 60) ` | ✅ | ✅ | ✅ | ✅ |
| **`build set_settings`**<br/>`object SetSettings(SetBuildSettingsInput settings = null, bool confirm = false, bool dryRun = false) ` | ✅ | ✅ | ✅ | ✅ |
| **`build status`**<br/>`JToken Status() ` | ✅ | ✅ | ✅ | ✅ |
| **`build.target switch`**<br/>`object Switch(string target = "", bool confirm = false, double timeoutInSeconds = 60) ` | ✅ | ✅ | ✅ | ✅ |
| **`build.target switch_status`**<br/>`JToken SwitchStatus() ` | ✅ | ✅ | ✅ | ✅ |
| **`capture screenshot`**<br/>`ScreenshotResponse Screenshot(string view = "game", string output = "", int width = 0, int height = 0) ` | ✅ | ✅ | ✅ | ✅ |
| **`component add`**<br/>`AuthoringResult Add(ObjectRef target, string type) ` | ✅ | ✅ | ✅ | ✅ |
| **`component get_fields`**<br/>`object GetFields(ObjectRef target, '). Omit to read all top-level fields.")] string field = null, string component = null, string detail = null) ` | ✅ | ✅ | ✅ | ✅ |
| **`component get_properties`**<br/>`ComponentPropertiesResult GetProperties(ObjectRef target, string type = null) ` | ✅ | ✅ | ✅ | ✅ |
| **`component remove`**<br/>`AuthoringResult Remove(ObjectRef target, string type = null) ` | ✅ | ✅ | ✅ | ✅ |
| **`component set_properties`**<br/>`ComponentPropertiesResult SetProperties(ObjectRef target, JObject properties, string type = null) ` | ✅ | ✅ | ✅ | ✅ |
| **`console clear`**<br/>`object Clear() ` | ✅ | ✅ | ✅ | ✅ |
| **`console get`**<br/>`ConsoleLogResponse Get(int tail = DefaultTail, string level = ConsoleLogBuffer.LevelLog, long since = -1, string since_session = null) ` | ✅ | ✅ | ✅ | ✅ |
| **`console status`**<br/>`ConsoleLogResponse Status() ` | ✅ | ✅ | ✅ | ✅ |
| **`console write`**<br/>`string Write(string message, string level = "info") ` | ✅ | ✅ | ✅ | ✅ |
| **`diagnostics get_performance`**<br/>`PerformanceStats GetPerformance() ` | ✅ | ✅ | ✅ | ✅ |
| **`diagnostics profiler_connect`**<br/>`object ProfilerConnect(string target = "") ` | ✅ | ✅ | ✅ | ✅ |
| **`diagnostics profiler_targets`**<br/>`object ProfilerTargets() ` | ✅ | ✅ | ✅ | ✅ |
| **`diagnostics recent_analytics`**<br/>`SentEvent> RecentAnalytics() ` | ✅ | ✅ | ✅ | ✅ |
| **`diagnostics record_analytics`**<br/>`string RecordAnalytics() ` | ✅ | ✅ | ✅ | ✅ |
| **`diagnostics report_evals`**<br/>`EvalUsageReport ReportEvals(int top = 50) ` | ✅ | ✅ | ✅ | ✅ |
| **`editor focus`**<br/>`string Focus() ` | ✅ | ✅ | ✅ | ✅ |
| **`editor invoke_menu`**<br/>`MenuResponse InvokeMenu(string path = "") ` | ✅ | ✅ | ✅ | ✅ |
| **`editor quit`**<br/>`EditorQuitResult Quit(bool save = false, bool discard = false, int exitCode = 0) ` | ✅ | ✅ | ✅ | ✅ |
| **`editor set_auto_tick`**<br/>`string SetAutoTick(bool enabled = true, int intervalMs = DefaultIntervalMs, bool persist = true) ` | ✅ | ✅ | ✅ | ✅ |
| **`editor status`**<br/>`StatusResponse Status() ` | ✅ | ✅ | ✅ | ✅ |
| **`editor.playmode add_player`**<br/>`object AddPlayer(string path = null, string type = null, string name = null) ` | ✅ | ✅ | ✅ | ✅ |
| **`editor.playmode list_instances`**<br/>`object ListInstances() ` | ✅ | ✅ | ✅ | ✅ |
| **`editor.playmode list_players`**<br/>`object ListPlayers(string path = null) ` | ✅ | ✅ | ✅ | ✅ |
| **`editor.playmode pause`**<br/>`string Pause() ` | ✅ | ✅ | ✅ | ✅ |
| **`editor.playmode remove_player`**<br/>`object RemovePlayer(string id, string path = null, bool confirm = false, bool dryRun = false) ` | ✅ | ✅ | ✅ | ✅ |
| **`editor.playmode set_run_in_background`**<br/>`string SetRunInBackground(bool enabled = true) ` | ✅ | ✅ | ✅ | ✅ |
| **`editor.playmode start`**<br/>`string Start() ` | ✅ | ✅ | ✅ | ✅ |
| **`editor.playmode stop`**<br/>`string Stop() ` | ✅ | ✅ | ✅ | ✅ |
| **`gameobject create`**<br/>`AuthoringResult Create(string name = null, string primitive = null, ObjectRef parent = null) ` | ✅ | ✅ | ✅ | ✅ |
| **`gameobject delete`**<br/>`AuthoringResult Delete(ObjectRef target) ` | ✅ | ✅ | ✅ | ✅ |
| **`gameobject find`**<br/>`FindGameObjectsResult Find(string name = null, string tag = null, string type = null, string hierarchyPath = null, bool includeInactive = true) ` | ✅ | ✅ | ✅ | ✅ |
| **`gameobject get_selection`**<br/>`SelectionResult GetSelection() ` | ✅ | ✅ | ✅ | ✅ |
| **`gameobject rename`**<br/>`AuthoringResult Rename(ObjectRef target, string name) ` | ✅ | ✅ | ✅ | ✅ |
| **`gameobject search`**<br/>`SearchResult Search(string query, int limit = 50) ` | ✅ | ✅ | ✅ | ✅ |
| **`gameobject set_active`**<br/>`AuthoringResult SetActive(ObjectRef target, bool active) ` | ✅ | ✅ | ✅ | ✅ |
| **`gameobject set_layer`**<br/>`AuthoringResult SetLayer(ObjectRef target, string layer) ` | ✅ | ✅ | ✅ | ✅ |
| **`gameobject set_parent`**<br/>`AuthoringResult SetParent(ObjectRef target, ObjectRef parent = null, bool worldPositionStays = true) ` | ✅ | ✅ | ✅ | ✅ |
| **`gameobject set_selection`**<br/>`SelectionResult SetSelection(ObjectId[] instanceIds = null, string[] paths = null) ` | ✅ | ✅ | ✅ | ✅ |
| **`gameobject set_tag`**<br/>`AuthoringResult SetTag(ObjectRef target, string tag) ` | ✅ | ✅ | ✅ | ✅ |
| **`gameobject set_transform`**<br/>`AuthoringResult SetTransform(ObjectRef target, .")] float[] position = null, in degrees.")] float[] rotation = null, .")] float[] scale = null) ` | ✅ | ✅ | ✅ | ✅ |
| **`job batch`**<br/>`BatchResult Batch() ` | ✅ | ✅ | ✅ | ✅ |
| **`job wait`**<br/>`object Wait() ` | ✅ | ✅ | ✅ | ✅ |
| **`job wait_cancel`**<br/>`object WaitCancel() ` | ✅ | ✅ | ✅ | ✅ |
| **`job wait_status`**<br/>`object WaitStatus() ` | ✅ | ✅ | ✅ | ✅ |
| **`lighting bake`**<br/>`object Bake(bool confirm = false, bool dryRun = false) ` | ✅ | ✅ | ✅ | ✅ |
| **`lighting bake_status`**<br/>`JToken BakeStatus() ` | ✅ | ✅ | ✅ | ✅ |
| **`lighting cancel_bake`**<br/>`object CancelBake() ` | ✅ | ✅ | ✅ | ✅ |
| **`lighting clear_bake`**<br/>`object ClearBake(bool confirm = false, bool includeDiskCache = false, bool dryRun = false) ` | ✅ | ✅ | ✅ | ✅ |
| **`lighting get_settings`**<br/>`LightingSettingsResult GetSettings() ` | ✅ | ✅ | ✅ | ✅ |
| **`lighting set_settings`**<br/>`object SetSettings(JObject settings, bool dryRun = false) ` | ✅ | ✅ | ✅ | ✅ |
| **`material get_shader_properties`**<br/>`ShaderPropertiesResult GetShaderProperties(string shader = null, ObjectRef material = null) ` | ✅ | ✅ | ✅ | ✅ |
| **`navmesh bake`**<br/>`object Bake(bool confirm = false, bool dryRun = false) ` | ✅ | ✅ | ✅ | ✅ |
| **`navmesh bake_status`**<br/>`JToken BakeStatus() ` | ✅ | ✅ | ✅ | ✅ |
| **`navmesh cancel_bake`**<br/>`object CancelBake() ` | ✅ | ✅ | ✅ | ✅ |
| **`navmesh clear_bake`**<br/>`object ClearBake(bool confirm = false, bool dryRun = false) ` | ✅ | ✅ | ✅ | ✅ |
| **`navmesh get_settings`**<br/>`NavMeshSettingsResult GetSettings() ` | ✅ | ✅ | ✅ | ✅ |
| **`navmesh set_settings`**<br/>`object SetSettings(JObject settings, bool dryRun = false) ` | ✅ | ✅ | ✅ | ✅ |
| **`occlusion bake`**<br/>`object Bake(float smallestOccluder = Unset, float smallestHole = Unset, float backfaceThreshold = Unset, bool confirm = false, bool dryRun = false) ` | ✅ | ✅ | ✅ | ✅ |
| **`occlusion bake_status`**<br/>`JToken BakeStatus() ` | ✅ | ✅ | ✅ | ✅ |
| **`occlusion cancel_bake`**<br/>`object CancelBake() ` | ✅ | ✅ | ✅ | ✅ |
| **`occlusion clear_bake`**<br/>`object ClearBake(bool confirm = false, bool dryRun = false) ` | ✅ | ✅ | ✅ | ✅ |
| **`package add`**<br/>`object Add(string identifier = "", bool confirm = false, bool dryRun = false, bool wait = false, double timeoutInSeconds = 300) ` | ✅ | ✅ | ✅ | ✅ |
| **`package list`**<br/>`object List(string scope = "installed",) ` | ✅ | ✅ | ✅ | ✅ |
| **`package remove`**<br/>`object Remove(string name = "", bool confirm = false, bool dryRun = false, bool wait = false, double timeoutInSeconds = 300) ` | ✅ | ✅ | ✅ | ✅ |
| **`package resolve`**<br/>`object Resolve() ` | ✅ | ✅ | ✅ | ✅ |
| **`package search`**<br/>`object Search(string query = "", bool offline = false, double timeoutInSeconds = 60) ` | ✅ | ✅ | ✅ | ✅ |
| **`package status`**<br/>`JToken Status() ` | ✅ | ✅ | ✅ | ✅ |
| **`pipeline list_commands`**<br/>`object ListCommands(string detail = null, string query = null, string group = null, string tag = null, bool includeTests = false, string groupBy = null, string sort = null, string order = null, int? offset = null, int? limit = null) ` | ✅ | ✅ | ✅ | ✅ |
| **`pipeline progress`**<br/>`object Progress() ` | ✅ | ✅ | ✅ | ✅ |
| **`prefab create`**<br/>`AuthoringResult Create(ObjectRef source, string path) ` | ✅ | ✅ | ✅ | ✅ |
| **`prefab instantiate`**<br/>`AuthoringResult Instantiate(ObjectRef prefab, string scenePath = null, string name = null) ` | ✅ | ✅ | ✅ | ✅ |
| **`prefab revert_overrides`**<br/>`AuthoringResult RevertOverrides(ObjectRef instance) ` | ✅ | ✅ | ✅ | ✅ |
| **`prefab unpack`**<br/>`AuthoringResult Unpack(ObjectRef instance, bool completely = false) ` | ✅ | ✅ | ✅ | ✅ |
| **`runtime quit`**<br/>`string Quit(int exitCode = 0) ` | ✅ | ✅ | ✅ | ✅ |
| **`runtime set_target_frame_rate`**<br/>`string SetTargetFrameRate(int frameRate) ` | ✅ | ✅ | ✅ | ✅ |
| **`runtime set_time_scale`**<br/>`string SetTimeScale(float scale) ` | ✅ | ✅ | ✅ | ✅ |
| **`runtime simulate_key`**<br/>`InputSimulationResponse SimulateKey(string key, string action = "press") ` | ✅ | ✅ | ✅ | ✅ |
| **`runtime simulate_pointer`**<br/>`InputSimulationResponse SimulatePointer(float x, float y, string action = "click", string button = "left") ` | ✅ | ✅ | ✅ | ✅ |
| **`runtime status`**<br/>`RuntimeStatusResponse Status() ` | ✅ | ✅ | ✅ | ✅ |
| **`scene add_to_build`**<br/>`object AddToBuild(string path, bool enabled = true) ` | ✅ | ✅ | ✅ | ✅ |
| **`scene create`**<br/>`AuthoringResult Create(string path, bool additive = false, string template = TemplateEmpty) ` | ✅ | ✅ | ✅ | ✅ |
| **`scene get_hierarchy`**<br/>`SceneHierarchy GetHierarchy(string path = null) ` | ✅ | ✅ | ✅ | ✅ |
| **`scene list_open`**<br/>`object ListOpen() ` | ✅ | ✅ | ✅ | ✅ |
| **`scene open`**<br/>`AuthoringResult Open(string path, bool additive = false) ` | ✅ | ✅ | ✅ | ✅ |
| **`scene remove_from_build`**<br/>`object RemoveFromBuild(string path) ` | ✅ | ✅ | ✅ | ✅ |
| **`scene save`**<br/>`AuthoringResult Save(string path = null) ` | ✅ | ✅ | ✅ | ✅ |
| **`scene save_all`**<br/>`object SaveAll() ` | ✅ | ✅ | ✅ | ✅ |
| **`scene set_active`**<br/>`AuthoringResult SetActive(string path) ` | ✅ | ✅ | ✅ | ✅ |
| **`script attach`**<br/>`AuthoringResult Attach(ObjectRef target, string type = null, string script = null) ` | ✅ | ✅ | ✅ | ✅ |
| **`script create`**<br/>`AuthoringResult Create(string name, string path = null, string @namespace = null, string baseClass = "MonoBehaviour", bool overwrite = false) ` | ✅ | ✅ | ✅ | ✅ |
| **`script eval`**<br/>`async Task<EvalResponse> Eval(string code = null, string file = null, double timeoutInSeconds = 30, bool dryRun = false, bool autoImport = false, bool internals = false) ` | ✅ | ✅ | ✅ | ✅ |
| **`script eval_file`**<br/>`async Task<EvalResponse> EvalFile(string file, double timeoutInSeconds = 30, bool dryRun = false, bool autoImport = false, bool internals = false) ` | ✅ | ✅ | ✅ | ✅ |
| **`script recompile`**<br/>`object Recompile(bool focus = false) ` | ✅ | ✅ | ✅ | ✅ |
| **`script recompile_status`**<br/>`RecompileStatusPayload RecompileStatus() ` | ✅ | ✅ | ✅ | ✅ |
| **`script run`**<br/>`async Task<RunScriptResponse> Run(string file, string entry = null, , and a string/object handle (ObjectRef) for UnityEngine.Object parameters. Ephemeral mode only (rejected with mode=hotpatch).")] JArray args = null, method replacements to already-loaded types (" + CommandNames.ScriptMethodreloadReload + " semantics).")] string mode = "ephemeral", string[] references = null, string[] defines = null, bool pdb = false, double timeoutInSeconds = 60, bool dryRun = false) ` | ✅ | ✅ | ✅ | ✅ |
| **`script.methodreload cleanup`**<br/>`MethodReloadResponse Cleanup(string assemblyDir, bool forceDomainReload = true) ` | ✅ | ✅ | ✅ | ✅ |
| **`script.methodreload reload`**<br/>`MethodReloadResponse Reload( methods (e.g. Assets/Scripts/Player.cs)", Required = true)] string filename, double timeoutInSeconds = 60, string assemblyDir = null, bool pdb = false) ` | ✅ | ✅ | ✅ | ✅ |
| **`script.methodreload reload_editor`**<br/>`MethodReloadResponse ReloadEditor( methods (e.g. Assets/Scripts/Player.cs)", Required = true)] string filename, double timeoutInSeconds = 60, string assemblyDir = null, bool pdb = false) ` | ✅ | ✅ | ✅ | ✅ |
| **`script.methodreload status`**<br/>`MethodReloadResponse Status() ` | ✅ | ✅ | ✅ | ✅ |
| **`settings. get`**<br/>`ProjectSettingsResponse Get() => ProjectSettingsCommand.Get(Domain, Read) ` | ✅ | ✅ | ✅ | ✅ |
| **`settings. set`**<br/>`ProjectSettingsResponse Set(AudioSettingsInput settings = null, bool confirm = false, bool dryRun = false) ` | ✅ | ✅ | ✅ | ✅ |
| **`test cancel`**<br/>`object Cancel() ` | ✅ | ✅ | ✅ | ✅ |
| **`test list`**<br/>`async Task<TestListResponse> List(string mode = "all") ` | ✅ | ✅ | ✅ | ✅ |
| **`test run`**<br/>`async Task<TestExecutionResponse> Run(string mode = "all", string filter = "", string filterType = "testName", attribute")] bool includeExplicit = false, bool asyncTests = false, double timeoutInSeconds = 600) ` | ✅ | ✅ | ✅ | ✅ |
| **`test status`**<br/>`JToken Status() ` | ✅ | ✅ | ✅ | ✅ |

---

## incompatible calls for Unity 2018.1

| apicall | newImplementation |
| :--- | :--- |
| **`animation.timeline add_clip`**<br/>`object AddClip(ObjectRef timeline, string track = null, float start = 0f, float duration = 0f, ObjectRef asset = null, bool dryRun = false) ` | `Unity.Pipeline.Adapters.Unity2018Adapter.AddClip(timeline, track, start, duration, asset, dryRun) ` |
| **`animation.timeline add_track`**<br/>`object AddTrack(ObjectRef timeline, string trackType = null, string name = null, string parentTrack = null, bool dryRun = false) ` | `Unity.Pipeline.Adapters.Unity2018Adapter.AddTrack(timeline, trackType, name, parentTrack, dryRun) ` |
| **`animation.timeline create`**<br/>`object Create(string path, float frameRate = 60f, bool confirm = false, bool dryRun = false) ` | `Unity.Pipeline.Adapters.Unity2018Adapter.Create(path, frameRate, confirm, dryRun) ` |
| **`animation.timeline get`**<br/>`object Get(ObjectRef timeline) ` | `Unity.Pipeline.Adapters.Unity2018Adapter.Get(timeline) ` |
| **`build create_profile`**<br/>`object CreateProfile(string name = "", string platform = "", string subtarget = "", string folder = "", bool activate = false, bool confirm = false, bool dryRun = false) ` | `Unity.Pipeline.Adapters.Unity2018Adapter.CreateProfile(name, platform, subtarget, folder, activate, confirm, dryRun) ` |
| **`build list_platforms`**<br/>`object ListPlatforms(bool showAll = false) ` | `Unity.Pipeline.Adapters.Unity2018Adapter.ListPlatforms(showAll) ` |
| **`build list_profiles`**<br/>`object ListProfiles() ` | `Unity.Pipeline.Adapters.Unity2018Adapter.ListProfiles() ` |
| **`build set_profile`**<br/>`object SetProfile(string name = "", string folder = "", bool? overrideGlobalScenes = null, string[] scenes = null, string[] scriptingDefines = null, bool confirm = false, bool dryRun = false) ` | `Unity.Pipeline.Adapters.Unity2018Adapter.SetProfile(name, folder, overrideGlobalScenes, scenes, scriptingDefines, confirm, dryRun) ` |
| **`capture editor_element`**<br/>`CaptureElementResponse EditorElement(string window = "", string selector = "", string output = "") ` | `Unity.Pipeline.Adapters.Unity2018Adapter.EditorElement(window, selector, output) ` |
| **`capture editor_window`**<br/>`CaptureResult EditorWindow(string window = null, string savePath = null, bool includeInlineImage = false, int maxResolution = 0) ` | `Unity.Pipeline.Adapters.Unity2018Adapter.EditorWindow(window, savePath, includeInlineImage, maxResolution) ` |
| **`capture game_view`**<br/>`CaptureResult GameView(int width = 1280, int height = 720, string camera = null, string savePath = null, bool includeInlineImage = false, int maxResolution = 0, string source = null, string overlay = null) ` | `Unity.Pipeline.Adapters.Unity2018Adapter.GameView(width, height, camera, savePath, includeInlineImage, maxResolution, source, overlay) ` |
| **`capture runtime_element`**<br/>`CaptureElementResponse RuntimeElement(string panel = "", string selector = "", string output = "") ` | `Unity.Pipeline.Adapters.Unity2018Adapter.RuntimeElement(panel, selector, output) ` |
| **`capture scene_view`**<br/>`CaptureResult SceneView(int width = DefaultWidth, int height = DefaultHeight, string savePath = null, bool includeInlineImage = false, int maxResolution = 0, string source = null) ` | `Unity.Pipeline.Adapters.Unity2018Adapter.SceneView(width, height, savePath, includeInlineImage, maxResolution, source) ` |
| **`diagnostics audit_status`**<br/>`JToken AuditStatus() ` | `Unity.Pipeline.Adapters.Unity2018Adapter.AuditStatus() ` |
| **`diagnostics memory_snapshot`**<br/>`object MemorySnapshot(string flags = "", string output = "", bool collectGarbage = true, double timeoutSeconds = DefaultTimeoutSeconds) ` | `Unity.Pipeline.Adapters.Unity2018Adapter.MemorySnapshot(flags, output, collectGarbage, timeoutSeconds) ` |
| **`diagnostics memory_snapshot_status`**<br/>`JToken MemorySnapshotStatus() ` | `Unity.Pipeline.Adapters.Unity2018Adapter.MemorySnapshotStatus() ` |
| **`diagnostics profiler_record`**<br/>`object ProfilerRecord(int frames = DefaultFrames, string output = "", bool deepProfile = false, double timeoutSeconds = DefaultTimeoutSeconds) ` | `Unity.Pipeline.Adapters.Unity2018Adapter.ProfilerRecord(frames, output, deepProfile, timeoutSeconds) ` |
| **`diagnostics profiler_record_status`**<br/>`JToken ProfilerRecordStatus() ` | `Unity.Pipeline.Adapters.Unity2018Adapter.ProfilerRecordStatus() ` |
| **`diagnostics profiler_record_stop`**<br/>`object ProfilerRecordStop() ` | `Unity.Pipeline.Adapters.Unity2018Adapter.ProfilerRecordStop() ` |
| **`diagnostics run_audit`**<br/>`object RunAudit(string categories = "", string output = "") ` | `Unity.Pipeline.Adapters.Unity2018Adapter.RunAudit(categories, output) ` |
| **`editor.playmode create_scenario`**<br/>`object CreateScenario(string name, string path = null) ` | `Unity.Pipeline.Adapters.Unity2018Adapter.CreateScenario(name, path) ` |
| **`editor.playmode get_scenario_status`**<br/>`object GetScenarioStatus() ` | `Unity.Pipeline.Adapters.Unity2018Adapter.GetScenarioStatus() ` |
| **`editor.playmode list_scenarios`**<br/>`object ListScenarios() ` | `Unity.Pipeline.Adapters.Unity2018Adapter.ListScenarios() ` |
| **`editor.playmode set_active_scenario`**<br/>`object SetActiveScenario(string path) ` | `Unity.Pipeline.Adapters.Unity2018Adapter.SetActiveScenario(path) ` |
| **`editor.playmode start_scenario`**<br/>`object StartScenario() ` | `Unity.Pipeline.Adapters.Unity2018Adapter.StartScenario() ` |
| **`editor.playmode stop_scenario`**<br/>`object StopScenario() ` | `Unity.Pipeline.Adapters.Unity2018Adapter.StopScenario() ` |
| **`multiplayer.role get_active`**<br/>`object GetActive() ` | `Unity.Pipeline.Adapters.Unity2018Adapter.GetActive() ` |
| **`multiplayer.role get_build`**<br/>`object GetBuild(string profile = null) ` | `Unity.Pipeline.Adapters.Unity2018Adapter.GetBuild(profile) ` |
| **`multiplayer.role get_component`**<br/>`object GetComponent(ObjectRef target, string type = null) ` | `Unity.Pipeline.Adapters.Unity2018Adapter.GetComponent(target, type) ` |
| **`multiplayer.role get_game_object`**<br/>`object GetGameObject(ObjectRef target) ` | `Unity.Pipeline.Adapters.Unity2018Adapter.GetGameObject(target) ` |
| **`multiplayer.role list_component_types`**<br/>`object ListComponentTypes() ` | `Unity.Pipeline.Adapters.Unity2018Adapter.ListComponentTypes() ` |
| **`multiplayer.role set_active`**<br/>`object SetActive(string role) ` | `Unity.Pipeline.Adapters.Unity2018Adapter.SetActive(role) ` |
| **`multiplayer.role set_build`**<br/>`object SetBuild(string role, string profile = null) ` | `Unity.Pipeline.Adapters.Unity2018Adapter.SetBuild(role, profile) ` |
| **`multiplayer.role set_component`**<br/>`object SetComponent(ObjectRef target, string role, string type = null) ` | `Unity.Pipeline.Adapters.Unity2018Adapter.SetComponent(target, role, type) ` |
| **`multiplayer.role set_component_type`**<br/>`object SetComponentType(string type, string role) ` | `Unity.Pipeline.Adapters.Unity2018Adapter.SetComponentType(type, role) ` |
| **`multiplayer.role set_game_object`**<br/>`object SetGameObject(ObjectRef target, string role) ` | `Unity.Pipeline.Adapters.Unity2018Adapter.SetGameObject(target, role) ` |
| **`navmesh bake_surfaces`**<br/>`object BakeSurfaces() ` | `Unity.Pipeline.Adapters.Unity2018Adapter.BakeSurfaces() ` |
| **`prefab apply_overrides`**<br/>`AuthoringResult ApplyOverrides(ObjectRef instance) ` | `Unity.Pipeline.Adapters.Unity2018Adapter.ApplyOverrides(instance) ` |
| **`prefab create_variant`**<br/>`AuthoringResult CreateVariant(ObjectRef basePrefab, string path) ` | `Unity.Pipeline.Adapters.Unity2018Adapter.CreateVariant(basePrefab, path) ` |
| **`prefab save_contents`**<br/>`AuthoringResult SaveContents(ObjectRef prefab, string renameChild = null, string newName = null, string setActiveChild = null, bool active = true) ` | `Unity.Pipeline.Adapters.Unity2018Adapter.SaveContents(prefab, renameChild, newName, setActiveChild, active) ` |

## incompatible calls for Unity 2019.4

| apicall | newImplementation |
| :--- | :--- |
| **`build create_profile`**<br/>`object CreateProfile(string name = "", string platform = "", string subtarget = "", string folder = "", bool activate = false, bool confirm = false, bool dryRun = false) ` | `Unity.Pipeline.Adapters.Unity2019Adapter.CreateProfile(name, platform, subtarget, folder, activate, confirm, dryRun) ` |
| **`build list_platforms`**<br/>`object ListPlatforms(bool showAll = false) ` | `Unity.Pipeline.Adapters.Unity2019Adapter.ListPlatforms(showAll) ` |
| **`build list_profiles`**<br/>`object ListProfiles() ` | `Unity.Pipeline.Adapters.Unity2019Adapter.ListProfiles() ` |
| **`build set_profile`**<br/>`object SetProfile(string name = "", string folder = "", bool? overrideGlobalScenes = null, string[] scenes = null, string[] scriptingDefines = null, bool confirm = false, bool dryRun = false) ` | `Unity.Pipeline.Adapters.Unity2019Adapter.SetProfile(name, folder, overrideGlobalScenes, scenes, scriptingDefines, confirm, dryRun) ` |
| **`capture editor_element`**<br/>`CaptureElementResponse EditorElement(string window = "", string selector = "", string output = "") ` | `Unity.Pipeline.Adapters.Unity2019Adapter.EditorElement(window, selector, output) ` |
| **`capture runtime_element`**<br/>`CaptureElementResponse RuntimeElement(string panel = "", string selector = "", string output = "") ` | `Unity.Pipeline.Adapters.Unity2019Adapter.RuntimeElement(panel, selector, output) ` |
| **`diagnostics audit_status`**<br/>`JToken AuditStatus() ` | `Unity.Pipeline.Adapters.Unity2019Adapter.AuditStatus() ` |
| **`diagnostics memory_snapshot`**<br/>`object MemorySnapshot(string flags = "", string output = "", bool collectGarbage = true, double timeoutSeconds = DefaultTimeoutSeconds) ` | `Unity.Pipeline.Adapters.Unity2019Adapter.MemorySnapshot(flags, output, collectGarbage, timeoutSeconds) ` |
| **`diagnostics memory_snapshot_status`**<br/>`JToken MemorySnapshotStatus() ` | `Unity.Pipeline.Adapters.Unity2019Adapter.MemorySnapshotStatus() ` |
| **`diagnostics profiler_record`**<br/>`object ProfilerRecord(int frames = DefaultFrames, string output = "", bool deepProfile = false, double timeoutSeconds = DefaultTimeoutSeconds) ` | `Unity.Pipeline.Adapters.Unity2019Adapter.ProfilerRecord(frames, output, deepProfile, timeoutSeconds) ` |
| **`diagnostics profiler_record_status`**<br/>`JToken ProfilerRecordStatus() ` | `Unity.Pipeline.Adapters.Unity2019Adapter.ProfilerRecordStatus() ` |
| **`diagnostics profiler_record_stop`**<br/>`object ProfilerRecordStop() ` | `Unity.Pipeline.Adapters.Unity2019Adapter.ProfilerRecordStop() ` |
| **`diagnostics run_audit`**<br/>`object RunAudit(string categories = "", string output = "") ` | `Unity.Pipeline.Adapters.Unity2019Adapter.RunAudit(categories, output) ` |
| **`editor.playmode create_scenario`**<br/>`object CreateScenario(string name, string path = null) ` | `Unity.Pipeline.Adapters.Unity2019Adapter.CreateScenario(name, path) ` |
| **`editor.playmode get_scenario_status`**<br/>`object GetScenarioStatus() ` | `Unity.Pipeline.Adapters.Unity2019Adapter.GetScenarioStatus() ` |
| **`editor.playmode list_scenarios`**<br/>`object ListScenarios() ` | `Unity.Pipeline.Adapters.Unity2019Adapter.ListScenarios() ` |
| **`editor.playmode set_active_scenario`**<br/>`object SetActiveScenario(string path) ` | `Unity.Pipeline.Adapters.Unity2019Adapter.SetActiveScenario(path) ` |
| **`editor.playmode start_scenario`**<br/>`object StartScenario() ` | `Unity.Pipeline.Adapters.Unity2019Adapter.StartScenario() ` |
| **`editor.playmode stop_scenario`**<br/>`object StopScenario() ` | `Unity.Pipeline.Adapters.Unity2019Adapter.StopScenario() ` |
| **`multiplayer.role get_active`**<br/>`object GetActive() ` | `Unity.Pipeline.Adapters.Unity2019Adapter.GetActive() ` |
| **`multiplayer.role get_build`**<br/>`object GetBuild(string profile = null) ` | `Unity.Pipeline.Adapters.Unity2019Adapter.GetBuild(profile) ` |
| **`multiplayer.role get_component`**<br/>`object GetComponent(ObjectRef target, string type = null) ` | `Unity.Pipeline.Adapters.Unity2019Adapter.GetComponent(target, type) ` |
| **`multiplayer.role get_game_object`**<br/>`object GetGameObject(ObjectRef target) ` | `Unity.Pipeline.Adapters.Unity2019Adapter.GetGameObject(target) ` |
| **`multiplayer.role list_component_types`**<br/>`object ListComponentTypes() ` | `Unity.Pipeline.Adapters.Unity2019Adapter.ListComponentTypes() ` |
| **`multiplayer.role set_active`**<br/>`object SetActive(string role) ` | `Unity.Pipeline.Adapters.Unity2019Adapter.SetActive(role) ` |
| **`multiplayer.role set_build`**<br/>`object SetBuild(string role, string profile = null) ` | `Unity.Pipeline.Adapters.Unity2019Adapter.SetBuild(role, profile) ` |
| **`multiplayer.role set_component`**<br/>`object SetComponent(ObjectRef target, string role, string type = null) ` | `Unity.Pipeline.Adapters.Unity2019Adapter.SetComponent(target, role, type) ` |
| **`multiplayer.role set_component_type`**<br/>`object SetComponentType(string type, string role) ` | `Unity.Pipeline.Adapters.Unity2019Adapter.SetComponentType(type, role) ` |
| **`multiplayer.role set_game_object`**<br/>`object SetGameObject(ObjectRef target, string role) ` | `Unity.Pipeline.Adapters.Unity2019Adapter.SetGameObject(target, role) ` |

## incompatible calls for Unity 2022.3

| apicall | newImplementation |
| :--- | :--- |
| **`build create_profile`**<br/>`object CreateProfile(string name = "", string platform = "", string subtarget = "", string folder = "", bool activate = false, bool confirm = false, bool dryRun = false) ` | `Unity.Pipeline.Adapters.Unity2022Adapter.CreateProfile(name, platform, subtarget, folder, activate, confirm, dryRun) ` |
| **`build list_platforms`**<br/>`object ListPlatforms(bool showAll = false) ` | `Unity.Pipeline.Adapters.Unity2022Adapter.ListPlatforms(showAll) ` |
| **`build list_profiles`**<br/>`object ListProfiles() ` | `Unity.Pipeline.Adapters.Unity2022Adapter.ListProfiles() ` |
| **`build set_profile`**<br/>`object SetProfile(string name = "", string folder = "", bool? overrideGlobalScenes = null, string[] scenes = null, string[] scriptingDefines = null, bool confirm = false, bool dryRun = false) ` | `Unity.Pipeline.Adapters.Unity2022Adapter.SetProfile(name, folder, overrideGlobalScenes, scenes, scriptingDefines, confirm, dryRun) ` |
| **`capture editor_element`**<br/>`CaptureElementResponse EditorElement(string window = "", string selector = "", string output = "") ` | `Unity.Pipeline.Adapters.Unity2022Adapter.EditorElement(window, selector, output) ` |
| **`capture runtime_element`**<br/>`CaptureElementResponse RuntimeElement(string panel = "", string selector = "", string output = "") ` | `Unity.Pipeline.Adapters.Unity2022Adapter.RuntimeElement(panel, selector, output) ` |
| **`editor.playmode create_scenario`**<br/>`object CreateScenario(string name, string path = null) ` | `Unity.Pipeline.Adapters.Unity2022Adapter.CreateScenario(name, path) ` |
| **`editor.playmode get_scenario_status`**<br/>`object GetScenarioStatus() ` | `Unity.Pipeline.Adapters.Unity2022Adapter.GetScenarioStatus() ` |
| **`editor.playmode list_scenarios`**<br/>`object ListScenarios() ` | `Unity.Pipeline.Adapters.Unity2022Adapter.ListScenarios() ` |
| **`editor.playmode set_active_scenario`**<br/>`object SetActiveScenario(string path) ` | `Unity.Pipeline.Adapters.Unity2022Adapter.SetActiveScenario(path) ` |
| **`editor.playmode start_scenario`**<br/>`object StartScenario() ` | `Unity.Pipeline.Adapters.Unity2022Adapter.StartScenario() ` |
| **`editor.playmode stop_scenario`**<br/>`object StopScenario() ` | `Unity.Pipeline.Adapters.Unity2022Adapter.StopScenario() ` |
| **`multiplayer.role get_active`**<br/>`object GetActive() ` | `Unity.Pipeline.Adapters.Unity2022Adapter.GetActive() ` |
| **`multiplayer.role get_build`**<br/>`object GetBuild(string profile = null) ` | `Unity.Pipeline.Adapters.Unity2022Adapter.GetBuild(profile) ` |
| **`multiplayer.role get_component`**<br/>`object GetComponent(ObjectRef target, string type = null) ` | `Unity.Pipeline.Adapters.Unity2022Adapter.GetComponent(target, type) ` |
| **`multiplayer.role get_game_object`**<br/>`object GetGameObject(ObjectRef target) ` | `Unity.Pipeline.Adapters.Unity2022Adapter.GetGameObject(target) ` |
| **`multiplayer.role list_component_types`**<br/>`object ListComponentTypes() ` | `Unity.Pipeline.Adapters.Unity2022Adapter.ListComponentTypes() ` |
| **`multiplayer.role set_active`**<br/>`object SetActive(string role) ` | `Unity.Pipeline.Adapters.Unity2022Adapter.SetActive(role) ` |
| **`multiplayer.role set_build`**<br/>`object SetBuild(string role, string profile = null) ` | `Unity.Pipeline.Adapters.Unity2022Adapter.SetBuild(role, profile) ` |
| **`multiplayer.role set_component`**<br/>`object SetComponent(ObjectRef target, string role, string type = null) ` | `Unity.Pipeline.Adapters.Unity2022Adapter.SetComponent(target, role, type) ` |
| **`multiplayer.role set_component_type`**<br/>`object SetComponentType(string type, string role) ` | `Unity.Pipeline.Adapters.Unity2022Adapter.SetComponentType(type, role) ` |
| **`multiplayer.role set_game_object`**<br/>`object SetGameObject(ObjectRef target, string role) ` | `Unity.Pipeline.Adapters.Unity2022Adapter.SetGameObject(target, role) ` |

## incompatible calls for Unity 6000.0+

*All commands natively compatible or handled within Core DLL.*

