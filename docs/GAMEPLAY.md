# L1_HelloFriends — gameplay script (HARD LOCK v1)

**Design freeze:** Hello Friends L1–L3 scripts are content-frozen. Two-week prototype **acceptance is still only this L1 one-level loop**, including `bye_required_bunny` / `bye_required_fox`. L2 (friend/me/you) and L3 (happy/sad) exist as id stubs only — see [LEVEL_TYPES.md](LEVEL_TYPES.md).

Playable loop on `HelloFriendsLevel` / `LevelController`. Timeline seconds are an **authoring guide**; the runtime is a sequenced state machine (`L1Beat`), not a wall-clock timer.

**Ignore any gameplay v2 that expands L1 to full-unit vocabulary.** HelloFriends must not present `me`, `you`, `happy`, `sad`, or `name` (nor Hi/Bye, Happy/Sad, Play 2×2, or Friend/Me/You grids). Those stay in [LEVEL_TYPES.md](LEVEL_TYPES.md) as later level types.

This is the **only** HelloFriends scene content.

## Success criteria (one run)

1. Three-choice tap: **Bunny / Tree / Rock** — pick **Bunny** for **Hi** (`choiceId=hi`, `optionId=bunny`, `correct=true`)
2. Drag the **ball** onto **Fox** for **Come play!** (`choiceId=come_play`, `optionId=fox`, `correct=true`) — same `OnChoice` shape as a tap
3. Optional speak-along **Hi** with `blockProgress=false` (Skip or ignore; level already clears after the invite)

Bye is **not** a success criterion. After optional rec the level plays **two** required demos (`assess=false`), with **no tap, no scoring, no fail path**, and never holds progress:

1. `bye_required_bunny` — Bunny waves + Bye!
2. `bye_required_fox` — Fox waves + Bye!

Main path: **Who says Hi? → Come play! → optional Hi! → Bye! (Bunny) → Bye! (Fox) → done**

## Beats

| Guide (s) | Beat | Child does | Phase |
| --- | --- | --- | --- |
| 0 | Intro | Listen. Friend first-look highlight. Status: Who says Hi? | none |
| 10 | Demo_Hi | Listen-only `clipId=hi`, `assess=false` | none |
| 25 | Choice_Hi | Tap Bunny (Tree / Rock). Assessed. | **1 — three-choice only** |
| 45 | Demo_Come | Fox appears. Listen-only `clipId=come_play`, `assess=false` | none |
| 65 | Task_Invite | `hit_ball` → `drop_fox` (snap 64). After 2 misses, tap-ball hint. | **2 — ball + Fox only** |
| 100 | Optional_Rec | Speak-along `utteranceId=hi`, does not block clear | none |
| 110 | ByeBunny | `bye_required_bunny` `assess=false`. Bunny wave. No taps. | wrap-up |
| 115 | ByeFox | `bye_required_fox` `assess=false`. Fox wave. No taps. | wrap-up |
| 120 | Outro | done. Progress already `completed` | wrap-up |

C#: `KidsEnglish.Levels.L1Beat`, `L1Phase`, ids on `L1HelloFriends`.

## Park gate scene (公园门口)

Reference canvas **390×844** (1 unit = 1 logical px):

| Id | Role | Size (px) |
| --- | --- | --- |
| `hit_bunny` | Hi choice | **96×140** |
| `hit_tree` | Hi distractor | **88×120** |
| `hit_rock` | Hi distractor | **88×88** |
| `hit_ball` | Come-play drag | **80×80** |
| `drop_fox` | Come-play drop | **100×120**, snap radius **64** |

Three-choice spacing **≥24**. Layout is computed so the 96+24+88+24+88 row fits the stage with side pad.

After **2 consecutive** misses (not on Fox), **tap-ball hint mode**.

Placeholder art (overwrite in `Assets/Art/`): `bg_park_gate`, `char_bunny`, `char_fox`, `prop_tree`, `prop_rock`, `prop_ball`. Anims: `anim_bunny_wave`, `anim_wave_bunny`, `anim_fox_wave`, `anim_fox_catch`. Do not add extra hotspot ids.

## Characters / hotspots

- **Phase 1:** only Bunny / Tree / Rock enabled
- **Phase 2:** only `hit_ball` + `drop_fox` enabled (three-choice stays visible as scenery, not tappable / not drop targets)
- **Wrap-up:** `bye_required_bunny` then `bye_required_fox`; no tap choice, no scoring, no fail path

Reuse: `ui_btn_listen`, `SpeakFeedback` (optional Hi!, retry/skip), `ui_parent_lock`, `progress_dots`. Official ids: [ASSETS.md](ASSETS.md).

## Scaffold

| Level | Hints |
| --- | --- |
| 1 | Full: labels + pulse on the target + Listen emphasis |
| 2 | Reduced: short labels, no pulse |
| 3 | Withdrawn: no labels, no pulse |

Starts at 1. Correct → +1 (cap 3). Wrong → back to 1. `OnFeedback.scaffoldLevel` reports the value **after** that adjustment.

Wrong = shake + warm orange + retry + emphasize Listen. No red X, crying face, or countdown.

## Events

See [EVENTS.md](EVENTS.md). Drag-drop is normalized to `OnChoice`.
