# Unity Pipeline & CLI/MCP Unified Solution

Unified multi-LTS Unity Pipeline UPM package and AI Agent Skill.

| Supported Unity Versions |
| --- |
| Unity 2018.1 |
| Unity 2019.4 LTS |
| Unity 2022.3 LTS |
| Unity 6000.0+ |

## Quick Start
1. **Install the `unity-cli` agent skill**
   Open PowerShell and run:
   ```powershell
   irm https://raw.githubusercontent.com/SmittyWerbenJJ/unity-cli/main/install.ps1 | iex
   ```
   This downloads and runs the bootstrap script to set up the `unity-cli` skill for your AI agent.

2. **Your agent sets up your Unity project and connects to the live Editor.**

## Building from Source
```powershell
dotnet build com.sinematic3d.pipeline/src~/Unity.Pipeline.Editor/Unity.Pipeline.Editor.csproj -c Release
```
