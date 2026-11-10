# Unity Pipeline & CLI/MCP Unified Solution (`com.unity.pipeline` + `unity-cli`)

Unified multi-LTS Unity Pipeline package (`0.10.0-lts.1`) and AI Agent Skill supporting **Unity 2018.1**, **Unity 2019.4 LTS**, **Unity 2022.3 LTS**, and **Unity 6000.0+**.

## Features
- **177 Native Engine Commands + IvanMurzak/Unity-MCP Compatibility Extensions** (`execute_menu_item`, `unity_reflect`, `get_sha`, `find_in_file`, `apply_text_edits`, `gameobject duplicate`, `scene frame`).
- **Unity 2018 Zero-Touch Bootstrapper** (`Editor/Bootstrap/PipelineBootstrap.cs` + `Newtonsoft.Json.zip`): Detects Unity version and checks if `Newtonsoft.Json` is already present and working in the project. If missing (e.g., on Unity 2018), automatically unpacks `Newtonsoft.Json.dll` into `Assets/Plugins/Newtonsoft.Json.dll` without relying on `.meta` `defineConstraints`.
- **Self-Contained Tracker Sync** (`scripts/sync_ivanmurzak_tracker.py`): Downloads and inspects `mcpforunityserver` releases inside `$TEMP/unity-mcp-sync` (never cluttering `~/.gemini/antigravity/mcp/`) and updates `IVANMURZAK_FEATURE_TRACKER.md`.

## Installation & Updating

### 1. As an Agent Skill (`~/.gemini/config/skills/unity-cli`)
```powershell
git clone https://github.com/SmittyWerbenJJ/unity-cli.git "$HOME\.gemini\config\skills\unity-cli"
```
To update both the skill, package binaries, and tracker:
```powershell
& "$HOME\.gemini\config\skills\unity-cli\scripts\install-pipeline-lts.ps1" -Update
```

### 2. In a Unity Project (UPM Git URL or Installer Script)
For **Unity 2019.4 / 2022.3 / 6000.0+**, add to `Packages/manifest.json`:
```json
{
  "dependencies": {
    "com.unity.pipeline": "https://github.com/SmittyWerbenJJ/unity-cli.git"
  }
}
```
Or run the installer script (which also auto-links `Assets/com.unity.pipeline` for **Unity 2018.1**):
```powershell
& "$HOME\.gemini\config\skills\unity-cli\scripts\install-pipeline-lts.ps1" -ProjectPath "G:\projects\MyProject"
```

## Building from Source (`src~/`)
```powershell
dotnet build src~/Unity.Pipeline/Unity.Pipeline.csproj -c Release
dotnet build src~/Unity.Pipeline.Editor/Unity.Pipeline.Editor.csproj -c Release
```
