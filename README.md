# Hello, Friends! — Kids English (Unity 2D)

引导式英语启蒙 prototype for ages **4–6**. First playable slice is **L1_HelloFriends**: listen and tap Bunny for “Hi”, drag the ball to Fox for “Come play!”, then optional speak-along. Not a quiz. Speak-along is a **local mic + scoring stub** only (`blockProgress=false`).

This repository is **Unity 6 ready**. Play Mode and the macOS `.app` were **not** run in the Linux cloud VM (no Unity Editor there). Open it on a Mac with **6000.6.1f1** and follow [docs/MAC_UNITY6.md](docs/MAC_UNITY6.md).

## Repo / path

- **Unity project root:** the folder that contains `Assets/`, `Packages/`, and `ProjectSettings/` (open that folder in Unity Hub).
- Mac copy: unzip `KidsEnglishL1-Unity6000.6.1f1-source.zip` (see `tools/pack_source_zip.sh`) to e.g. `~/Projects/KidsEnglishL1`.

## Unity version

| | Before (historical) | After (this branch) |
| --- | --- | --- |
| Editor | Unity 2022.3 LTS `2022.3.52f1` (`1f61eb05bbc0`) | **Unity 6** `6000.6.1f1` (`7efac9f6c10e`) |
| URP 2D | `com.unity.render-pipelines.universal` 14.0.11 | **17.6.0** (editor may remap to the 17.6.x patch bundled with 6000.6.1f1) |
| Addressables | 1.21.21 | **2.11.2** |
| UI | uGUI 1.0 + TMP 3.0.7 | **uGUI 2.0.0** (TMP is part of ugui 2; not a separate package) |
| Scripting backend (Standalone) | unset (Editor default) | **Mono** (`scriptingBackend.Standalone: 0`) |

- 2D: `com.unity.2d.sprite` (the old `com.unity.feature.2d` bundle is not required for L1)
- **Orientation:** portrait (`390×844` canvas, 1 unit = 1 logical px)
- macOS Dev player: **Mono is OK**. `KidsEnglish.Editor.MacDevBuild.Build` forces Mono when the IL2CPP module is missing.

Exact Mac open / Play / batch-build steps: [docs/MAC_UNITY6.md](docs/MAC_UNITY6.md).

## Temporary design tokens

Locked wireframe (no brand pack). Source: `KidsEnglish.Core.DesignTokens` and [docs/UI.md](docs/UI.md).

| Token | Value |
| --- | --- |
| bg | light sky blue `#A8D8F0` |
| primary | `#3B82F6` |
| success | grass green `#43A047` |
| soft-warn | warm orange `#F59E0B` |
| card | white `#FFFFFF` |
| text | near-black `#1A1A1A`, ≥20sp |

Motion: tap **150ms** (120–180), celebration **≤1.2s**. hitSlop may exceed the visual. Wrong = shake + orange + retry + emphasize Listen — **no** big red X, crying face, or countdown.

## Open the project (Mac, Unity 6000.6.1f1)

Full walkthrough: [docs/MAC_UNITY6.md](docs/MAC_UNITY6.md).

1. Install Unity Hub + **6000.6.1f1** (Apple Silicon) with **Mac Build Support (Mono)**. IL2CPP is not required.
2. Hub → Open → select this folder (the folder that contains `Assets/`, `Packages/`, `ProjectSettings/`).
3. Wait for package resolve. If URP/Addressables patch versions shift to the editor-bundled set, accept the safe upgrade.
4. Menu **Kids English → First Open Setup**
   - Writes Addressables groups including `Art_Scene` (park-gate + anim chips) and `Audio_L1` (vo_* stubs)
   - Marks Unit 1 slots (`hi`, `bye`, `friend`, … `hi_bye`, `come_play`) with addresses `Audio/{key}` and `Art/{key}`
   - Assigns `Assets/Settings/URP-2D-Pipeline.asset` as the default render pipeline
   - Sets Standalone scripting backend to **Mono**
   - Confirms Build Settings: `Bootstrap` then `HelloFriends`
5. Optional: **Kids English → Create SpeakFeedback Prefab** → `Assets/Prefabs/SpeakFeedback.prefab`
6. Open `Assets/Scenes/Bootstrap.unity` (or **Kids English → Open HelloFriends Scene**) → Play.

If the Game view is pink, the 2D renderer asset did not bind: **Edit → Project Settings → Graphics → Default Render Pipeline** = `URP-2D-Pipeline`, and that asset’s renderer list should contain `URP-2D-Renderer`.

## macOS Development Build (unsigned, local Dev, Mono)

Output: `Builds/macOS/KidsEnglishL1.app` (folder is created; `Builds/` is gitignored). Scenes: **Bootstrap** then **HelloFriends**. Flags: **Development | AllowDebugging**. Mac App Store validation is off — **no code signing** for local run. Scripting backend: **Mono** (forced when the IL2CPP module is missing).

Menu: **Kids English → Build macOS Development**

The build calls **First Open Setup** first (Addressables groups, URP, scene list, Mono backend). That path is cheap. If Addressables groups were never created in this Editor, run **Kids English → First Open Setup** once before a batch build, or let `MacDevBuild.Build` do it.

Batch (Mac Unity **6000.6.1f1**). Entry point: **`KidsEnglish.Editor.MacDevBuild.Build`**. Use `-buildTarget OSXUniversal` so the Editor does not domain-reload mid-method. Do **not** pass `-quit` — the method exits batchmode with the build result code.

```bash
UNITY="/Applications/Unity/Hub/Editor/6000.6.1f1/Unity.app/Contents/MacOS/Unity"
PROJECT="$(pwd)"   # folder that contains Assets/ Packages/ ProjectSettings/

"$UNITY" -batchmode -nographics \
  -projectPath "$PROJECT" \
  -buildTarget OSXUniversal \
  -executeMethod KidsEnglish.Editor.MacDevBuild.Build \
  -logFile -
```

Then open the unsigned app locally (`open Builds/macOS/KidsEnglishL1.app`, or right-click → Open if Gatekeeper blocks). First-time Hub installs may live under `~/Applications/Unity/Hub/Editor/6000.6.1f1/` instead.

## Gameplay (L1_HelloFriends) — HARD LOCK v1

First playable loop is **only** this. Ignore any script that expands L1 into full-unit vocab (`me` / `you` / `happy` / `sad` / `name`). Those files may exist as later Addressable slots; they are **not** HelloFriends tasks.

Sequenced loop (not a wall-clock timer). Full beat table: [docs/GAMEPLAY.md](docs/GAMEPLAY.md).

**Beats:** Intro (Friend highlight) → listen-only Hi → Who says Hi? (Bunny) → listen-only Come play → ball to Fox → optional Hi! → Bye! Bunny → Bye! Fox → done

1. Portrait **Hello, Friends!** Parent **🔒** (long-press, ≥64dp). Park-gate stage (`bg_park_gate`) on a **390×844** canvas.
2. Tap **Listen** (≥80dp). Listen-only **Hi** (`assess=false`).
3. Phase 1: `hit_bunny` 96×140 / `hit_tree` 88×120 / `hit_rock` 88×88, spacing ≥24. Tap Bunny.
4. Listen-only **Come play!** Phase 2: `hit_ball` 80×80 → `drop_fox` 100×120 (snap 64). Three-choice not enabled.
5. Level **clears** after the invite. Optional speak-along **Hi** (`blockProgress=false`).
6. Unassessed **Bye twice** (`clipId=bye_required_bunny` then `bye_required_fox`, `assess=false`): Bunny waves, then Fox waves. No tap, no score, no fail. Does **not** block clear. Then done.

Unit vocab grids (Hi/Bye, Happy/Sad, Play 2×2, Friend/Me/You) are **not** in this scene — [docs/LEVEL_TYPES.md](docs/LEVEL_TYPES.md). L2/L3 are reserved stubs only; this week ships L1.

Scaffold **1** full hints → **2** reduced → **3** withdrawn. Wrong: shake + warm orange + retry + Listen pulses. No red X.

Listen sub-states still: waiting-to-listen → playing → waiting-choice → correct / wrong.

`AudioPlayer` **stops the demo** when a choice or record starts so two voices never overlap.

## Parent mode

Corner **🔒**:

- **Long-press ~1.6s** → parent panel
- **Tap** → PIN pad (prototype default `0000`)

Panel: **会指认** and **敢跟读** lights only (● on / ○ not yet — never a red X). No accuracy rates or rankings. SFX and Speak-along toggles, delete local voice folder.

L1 this week lights 会指认 **Hi**, **Come play**, **Bye wave**, and 敢跟读 **Hi!** if the child speaks. Friend/Me/You/Happy/Sad and Bye!/Come play! speak lights are schema placeholders for later levels. Copy keys: `parent.identify.*`, `parent.speak.*`.

## Privacy (child data minimization)

- No accounts, no cloud save, no IAP, no face camera (`cameraUsageDescription` states unused).
- Progress: `persistentDataPath/kids_english_progress.json` — 会指认 / 敢跟读 booleans, level completed flag. No accuracy, no ranking.
- Voice: `persistentDataPath/voice_local/*.wav` — filename is level + prompt + unix time. Never uploaded. Parent can delete.
- Scoring: `StubSpeechScorer` implements `ISpeechScorer` in-process.

## Event contract

Exact names for 玩法 / 美术:

`OnPlayDemo` · `OnChoice` · `OnRecordStart` · `OnRecordStop` · `OnFeedback`

See [docs/EVENTS.md](docs/EVENTS.md) and [docs/GAMEPLAY.md](docs/GAMEPLAY.md). C# surface: `KidsEnglish.Core.GameEventBus` and `KidsEnglish.Levels.LevelController`.

Payloads (locked): `OnPlayDemo { clipId, targetId?, canReplay, assess }` · `OnChoice { choiceId, optionId, correct }` · `OnRecordStart { utteranceId }` · `OnRecordStop { utteranceId, audioRef?, score? }` · `OnFeedback { kind, textKey, scaffoldLevel? }`.

## Dropping art & audio

See [docs/ASSETS.md](docs/ASSETS.md) for the **official L1 P0 list** (exact Art ids). Unit 1 catalog keys (`hi`, `bye`, `friend`, … `hi_bye`, `come_play`) may coexist; HelloFriends does **not** run the 8-card quiz.

L1 drop-ins (overwrite, keep filename):

- Scene: `bg_park_gate`, `char_bunny`, `char_fox`, `prop_tree`, `prop_rock`, `prop_ball`
- Anims: `anim_bunny_wave`, `anim_wave_bunny`, `anim_fox_wave`, `anim_fox_catch`
- VO: `vo_who_says_hi`, `vo_come_play_hint`, `vo_hi_speak`, `vo_bye_expose`
- SFX: `sfx_star`, `sfx_wrong_soft`

OnPlayDemo **clipIds** stay `hi`, `come_play`, `bye_required_bunny`, `bye_required_fox` — do not rename events to `vo_*`.

then run **First Open Setup**. Addresses stay `Audio/{key}` and `Art/{key}` for scene/VO; SFX addresses are the bare id.

Runtime fallback order: Addressables → `Resources/` copies → generated beep / colored circle.

## Folder map

```
Assets/Scripts/Core        bootstrap, tokens, event bus, Addressables loader, UI helpers
Assets/Scripts/Audio       AudioPlayer, LocalMicRecorder, ISpeechScorer stub
Assets/Scripts/Input       character hotspots, ball drag, parent lock
Assets/Scripts/Progress    JSON store (`L1_HelloFriends`)
Assets/Scripts/Parent      PIN + panel
Assets/Scripts/Levels      L1_HelloFriends loop + SpeakFeedbackPanel
Assets/Scripts/Editor      First Open Setup + macOS Dev Build (`KidsEnglish.Editor.MacDevBuild.Build`)
docs/MAC_UNITY6.md         Mac: unzip, Hub open 6000.6.1f1, Play, batch-build .app
tools/pack_source_zip.sh   Clean source zip (no Library/Temp/Obj/Logs)
Assets/Prefabs             SpeakFeedback prefab (generate in Editor)
Assets/Settings            URP 2D pipeline + 2D renderer
Assets/Scenes              Bootstrap.unity, HelloFriends.unity
docs/ASSETS.md             official L1 P0 Art ids (exact)
docs/GAMEPLAY.md           locked L1 beat script
docs/LEVEL_TYPES.md        deferred unit-vocab layouts (not in L1)
docs/UI.md                 wireframe, tokens, states
docs/EVENTS.md             event payloads
Assets/Addressables/Levels/UnitVocab   empty hook for later grids
```

## Out of scope

Real ASR/pronunciation SDK, accounts, cloud save, IAP, store packaging, multi-chapter pipeline, and unit vocab mount layouts (Hi/Bye, Happy/Sad, Play 2×2, Friend/Me/You).

## License note

Placeholder PNGs/WAVs are generated geometric tones and color chips, not licensed character art.
