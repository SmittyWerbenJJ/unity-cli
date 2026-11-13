---
name: unity-cli
description: Controls the Unity Editor and projects from the command line, auto-installs com.sinematic3d.pipeline (Unity 2018.1–6000+), and drives a running Editor to edit scenes, assets, or run C#. Use when setting up a Unity project, installing or managing Editors/licenses/VCS, building, testing, configuring Unity MCP, or running any unity command.
---

# Unity CLI & Pipeline (`com.sinematic3d.pipeline`)

Use the `unity` CLI (paired with `com.sinematic3d.pipeline`, supporting **Unity 2018.1, 2019.4 LTS, 2022.3 LTS, and 6000.0+**) to inspect and drive Unity projects and live Editors.

---

## 1. Preflight: Ensure CLI & Package Are Ready

### A. Verify `unity` CLI
Check if `unity` is on `PATH` (`which unity && unity --version` or `Get-Command unity`). If missing:
- **Windows (PowerShell):** `irm https://unity.com/install.ps1 | iex`
- **macOS / Linux:** `curl -fsSL https://unity.com/install.sh | bash`

### B. Auto-Install `com.sinematic3d.pipeline` into the Target Project
Never ask the user to manually open Package Manager or run an installer script. When a target Unity project does not yet have `com.sinematic3d.pipeline`:
1. **Check project version** (`<ProjectPath>/ProjectSettings/ProjectVersion.txt`) and `Packages/manifest.json`.
2. **Unity 2019.4 LTS – 6000.0+:** Add `"com.sinematic3d.pipeline": "https://github.com/SmittyWerbenJJ/unity-cli.git?path=/com.sinematic3d.pipeline"` to `<ProjectPath>/Packages/manifest.json` (and remove any legacy `com.unity.pipeline` entry).
3. **Unity 2018.1 (or offline/local):** Copy `<skill-dir>/com.sinematic3d.pipeline/` directly into `<ProjectPath>/Packages/com.sinematic3d.pipeline/`.
4. **Confirm connection:** Once Unity loads the project, it starts the bridge on port `7800` (`.unity-pipeline-port`). Run `unity status --project-path <ProjectPath>` and confirm state `ready`.

---

## 2. Drive a Running Unity Editor (`unity status` First)

**Before editing any scene, GameObject, prefab, or asset, run `unity status`.** If a live Editor is reachable, drive it via `unity command` instead of hand-editing `.unity`, `.prefab`, or `.asset` YAML (raw YAML edits miss in-memory state, risk bad GUIDs/fileIDs, and may hit the wrong scene).

```bash
unity status --project-path <path>           # confirm state "ready"
unity command --project-path <path>          # list commands exposed by this Editor
unity command editor_play --project-path <path>
unity command eval 'new UnityEngine.GameObject("Joe");' --project-path <path>
```

- **Multiple Editors open?** Always pass `--project-path <full-path>` (or set `UNITY_PROJECT_PATH`). Target only the project you own.
- **Verifying Play Mode?** Entering Play Mode is setup, not proof; follow [references/playmode-verification-loop.md](references/playmode-verification-loop.md).
- **Safe Mode vs. Sandbox false negatives:**
  - If `unity status` fails to connect, run `unity pipeline list` to check for **Safe Mode** (caused by C# compile errors blocking package load). Fix the C# errors, restart Unity, and re-poll. See [references/integration-advanced.md](references/integration-advanced.md).
  - If running inside a restrictive sandbox, `unity status` may report no instances even when an Editor is open. Do not silently invent workarounds; inform the user before falling back to direct file edits.
- **Timeouts / crashes:** Report `errors[0].code` (`--format json`), what you ran, and what failed. Never run `unity bug` without asking first.

---

## 3. GUI Editor vs. Headless Batchmode Daemon

- **GUI Editor (Recommended):** `unity open <project>` or `Unity.exe -projectPath <path>`. Attaches the user's Unity Hub session (required for `assetstore_*` commands).
- **Headless Batchmode Daemon (CI / no GUI):** Launch with `-batchmode` and **OMIT `-quit`** so the Editor stays resident on port `7800`:
  ```powershell
  Start-Process "Unity.exe" -ArgumentList "-batchmode", "-projectPath", "`"$projectPath`"", "-logFile", "`"$projectPath\editor.log`""
  ```
  ```bash
  "$UNITY_PATH" -batchmode -projectPath /path/to/MyProject -logFile editor.log &
  ```

---

## 4. Global Flags, Output & Exit Codes

- **Output formats:** `--format json` (structured `data` / `errors`), `--format ndjson`, `--format table`, `--format plain`.
- **Non-interactive & scoping:** `--non-interactive`, `--project-path <path>`, `--timeout <duration>`, `--verbose`.
- **Common exit codes:** `0` success, `1` general error, `2` usage/invalid args, `3` auth required, `4` not found, `5` permission denied, `6` timeout / not ready, `7` Editor in Safe Mode or compile failure.
- Full flag, env var (`UNITY_PROJECT_PATH`, `UNITY_FORMAT`, `UNITY_CLI_NON_INTERACTIVE`), and exit code details: [references/diagnostics-maintenance.md](references/diagnostics-maintenance.md).

---

## 5. Command Reference Routing

Read the relevant reference file (one level deep) for full command syntax, flags, and examples:

| Domain / Commands | Reference |
|---|---|
| `auth`, `license`, `cloud` | [references/auth-license-cloud.md](references/auth-license-cloud.md) |
| `pipeline cloud-build`, `pipeline automation` | [references/cloud-automation.md](references/cloud-automation.md) |
| `editors`, `install`, `uninstall`, `modules`, `install-modules` | [references/editors-install.md](references/editors-install.md) |
| `projects`, `open`, `close`, `releases`, `templates`, `assets` | [references/projects-templates.md](references/projects-templates.md) |
| `config`, `context`, `hub install` | [references/config-hub.md](references/config-hub.md) |
| `run`, `test`, `build`, `recompile`, `watch` | [references/build-run-test.md](references/build-run-test.md) |
| `logs`, `doctor`, `env`, `version`, `cache`, `ci init`, `bug`, `self-update` | [references/diagnostics-maintenance.md](references/diagnostics-maintenance.md) |
| `mcp`, `skill`, `plugin`, `pipeline`, `command`, `status`, `list`, `job`, `shell` | [references/integration-advanced.md](references/integration-advanced.md) |
| Play Mode verification loop (`eval`, screenshots, console) | [references/playmode-verification-loop.md](references/playmode-verification-loop.md) |
| `vcs`, `vcs git`, `vcs uvcs` | [references/version-control.md](references/version-control.md) |
| `collaboration` / `collab` | [references/collaboration.md](references/collaboration.md) |
