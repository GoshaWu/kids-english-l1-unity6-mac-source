# Open and build on Mac — Unity 6000.6.1f1

This project is pinned to **Unity 6** (`6000.6.1f1`, changeset `7efac9f6c10e`) for Apple Silicon. L1 gameplay is unchanged (`hi` / `friend` / `come` / `play` + **Hi!** / **Come play!**; **Bye!** is required and not assessed).

Play Mode and the `.app` were **not** run in the Linux cloud VM (no Unity Editor there). Do the steps below on the Mac that has `6000.6.1f1` and **MacStandaloneSupport Mono**.

## Where to put the project

1. Copy the source zip (`KidsEnglishL1-Unity6000.6.1f1-source.zip`) to the Mac. Recreate it anytime with `tools/pack_source_zip.sh`.
2. Unzip to a path **without spaces if you can**, for example:
   - `~/Projects/KidsEnglishL1`
   - `~/dev/KidsEnglishL1`
3. The folder you open in Unity Hub must contain `Assets/`, `Packages/`, and `ProjectSettings/` at its root (this repo root). Do **not** open a parent folder or the zip itself.
4. Do not copy a `Library/`, `Temp/`, `Obj/`, or `Logs/` folder from another machine. Unity 6 will regenerate `Library/` on first open.

## Hub install (already on your Mac)

Confirm **Unity Hub → Installs**:

| Item | Expected |
| --- | --- |
| Editor | **6000.6.1f1** (Apple Silicon / ARM64) |
| Module | **Mac Build Support (Mono)** — `MacStandaloneSupport` |
| IL2CPP | Not required. The Dev build forces **Mono** if IL2CPP is missing. |

Editor binary (Hub default):

```text
/Applications/Unity/Hub/Editor/6000.6.1f1/Unity.app/Contents/MacOS/Unity
```

Some Hub installs live under `~/Applications/Unity/Hub/Editor/6000.6.1f1/` instead.

## Open in 6000.6.1f1

1. Quit any Unity Editor that already has this folder open.
2. Unity Hub → **Projects** → **Open** → select `KidsEnglishL1` (the folder with `Assets/`).
3. If Hub offers an editor version, choose **6000.6.1f1**. `ProjectSettings/ProjectVersion.txt` already names that version, so Hub should not treat this as a 2022.3 upgrade.
4. First open will resolve packages (`URP 17.6.x`, `Addressables 2.11.2`, `ugui 2.0`). Accept **safe / official** package upgrades if the Editor remaps a core package patch (for example `17.6.0` → the exact patch bundled with `6000.6.1f1`).
5. Let the script updater run if it offers to update API usages. The repo already uses `FindAnyObjectByType` and `GraphicsSettings.defaultRenderPipeline`.
6. Wait until the status bar is idle (no “importing” / “compiling”).
7. Menu **Kids English → First Open Setup**
   - Writes Addressables groups including `Art_Scene` and `Audio_L1`
   - Assigns `Assets/Settings/URP-2D-Pipeline.asset` as the default render pipeline
   - Sets Standalone scripting backend to **Mono**
   - Confirms Build Settings: `Bootstrap` then `HelloFriends`
8. Optional: **Kids English → Create SpeakFeedback Prefab**

If the Game view is pink after import: **Edit → Project Settings → Graphics → Default Render Pipeline** = `URP-2D-Pipeline`, and that asset’s renderer list should still contain `URP-2D-Renderer`. Then run **First Open Setup** again. If the URP asset itself failed to upgrade from the old URP 14 serialization, recreate **URP Asset (with 2D Renderer)** at the same two paths under `Assets/Settings/`.

## Play HelloFriends

1. **Kids English → Open HelloFriends Scene**, or open `Assets/Scenes/Bootstrap.unity`.
2. Set the Game view to a portrait size (the canvas is **390×844**).
3. Press **Play**.
4. Expected L1 loop (frozen): Intro → listen **Hi** → tap **Bunny** → listen **Come play!** → drag ball to **Fox** → optional **Hi!** speak-along → **Bye!** Bunny → **Bye!** Fox → done.

Bootstrap loads `HelloFriends` automatically. Either scene is a valid Play start.

## Build macOS Development → `KidsEnglishL1.app` (Mono)

Output: `Builds/macOS/KidsEnglishL1.app` (created on disk; `Builds/` is gitignored). Scenes: **Bootstrap** then **HelloFriends**. Flags: **Development | AllowDebugging**. Mac App Store validation is off — **no code signing**.

### Editor menu

**Kids English → Build macOS Development**

That menu runs First Open Setup, forces **Mono** if IL2CPP is missing, prefers **Apple silicon**, then `BuildPipeline.BuildPlayer`.

### Batch (`-executeMethod`)

Entry point: **`KidsEnglish.Editor.MacDevBuild.Build`**

Use `-buildTarget OSXUniversal` so the Editor does not domain-reload in the middle of the method. Do **not** pass `-quit` — the method exits batchmode with the build result code. The player architecture is then set to **Apple silicon** when that Editor API exists, so a Mono-only Apple Silicon module can still produce an `.app`.

```bash
UNITY="/Applications/Unity/Hub/Editor/6000.6.1f1/Unity.app/Contents/MacOS/Unity"
PROJECT="$HOME/Projects/KidsEnglishL1"   # folder that contains Assets/ Packages/ ProjectSettings/

"$UNITY" -batchmode -nographics \
  -projectPath "$PROJECT" \
  -buildTarget OSXUniversal \
  -executeMethod KidsEnglish.Editor.MacDevBuild.Build \
  -logFile -
```

If the Hub editor is under your home folder, swap `UNITY` for:

```bash
UNITY="$HOME/Applications/Unity/Hub/Editor/6000.6.1f1/Unity.app/Contents/MacOS/Unity"
```

Then open the unsigned app:

```bash
open "$PROJECT/Builds/macOS/KidsEnglishL1.app"
```

If Gatekeeper blocks it: right-click the `.app` → **Open**.

### If the batch build fails with IL2CPP / module missing

The Dev entry point calls `MacDevBuild.EnsureStandaloneMonoBackend()`. A remaining IL2CPP error usually means the Editor still has Standalone set to IL2CPP **and** the probe did not run (for example the method never started because of a domain reload). Fix:

1. Confirm `-buildTarget OSXUniversal` is on the command line.
2. Open the project once in the Editor, run **Kids English → First Open Setup**, then retry the batch command.
3. **Edit → Project Settings → Player → Other Settings → Scripting Backend** = **Mono**.

## Recreate the source zip

From a clone (no `Library/` required):

```bash
./tools/pack_source_zip.sh
```

Default output: `/opt/cursor/artifacts/KidsEnglishL1-Unity6000.6.1f1-source.zip` when that folder exists, otherwise a path you pass as the first argument.

The zip is `git archive` of HEAD: tracked source only. It never includes `Library/`, `Temp/`, `Obj/`, `Logs/`, `Builds/`, or `.git`.

## Residual risks (cloud Linux could not Play-test)

| Risk | What you may see | What to do |
| --- | --- | --- |
| First-open URP asset upgrade | Pink Game view; missing 2D renderer | Reassign `URP-2D-Pipeline`; recreate the 2D renderer asset if Unity refuses the old URP 14 YAML |
| Core package patch remap | Package Manager bumps `17.6.0` to the editor-bundled 17.6.x | Accept the official remap |
| Addressables 1.21 → 2.11 | First Open Setup recreates groups (none are committed) | Run **First Open Setup** once before Play or batch build |
| `OSXUniversal` vs Apple Silicon-only Mono | Rare “missing Intel player” / architecture error | The script sets Apple silicon when the OSX API is present; build from the Editor menu if batch still asks for Intel |
| Unsigned `.app` | Gatekeeper block | Right-click → Open |
| Speak-along mic | macOS permission prompt | Expected; recordings stay on device |
| Play Mode / `.app` not verified here | Cloud environment has no Unity 6000 Editor | Validate Play + menu/batch build on the Mac |

L1 scope stays frozen. Do not treat Unit 1 card keys (`me` / `you` / `happy` / `sad` / `name`) as HelloFriends tasks.
