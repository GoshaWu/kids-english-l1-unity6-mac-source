# UI wireframe (locked) — temporary tokens

Portrait 听辨 + 跟读 skeleton. Placeholders only; no brand pack required.
Canvas reference **390×844** so **1 unit = 1 logical px**.

## Tokens

| Token | Value |
| --- | --- |
| bg | light sky blue `#A8D8F0` |
| primary | `#3B82F6` |
| success | grass green `#43A047` |
| soft-warn | warm orange `#F59E0B` |
| card | white `#FFFFFF` |
| text | near-black `#1A1A1A`, **≥20sp** |

C#: `KidsEnglish.Core.DesignTokens`.

## Motion

- Tap scale: **150ms** (within 120–180ms)
- Celebration (star pop): **≤1.15s** (cap 1.2s)
- Wrong: horizontal shake, then warm-orange overlay — **no** big red X, crying face, or countdown
- `hitSlop` **12dp** beyond the visual on Listen, mic, parent lock, ball, and big buttons

## L1_HelloFriends stage — park gate

| Spec | Implementation |
| --- | --- |
| Parent lock 🔒, long-press, ≥64dp | `ui_parent_lock` (`ParentEntryButton`), visual 64dp + slop |
| Listen-demo ≥80dp | `ui_btn_listen`, visual 80dp |
| Progress | `progress_dots` (four dots) |
| `hit_bunny` | **96×140** |
| `hit_tree` | **88×120** |
| `hit_rock` | **88×88**, three-choice spacing ≥24 |
| `drop_fox` | **100×120**; snap 64 or tap-to-send |
| `hit_ball` | **80×80**; bounce on miss; tap-hint after 2 fails |
| Phase lock | Phase 1 = three-choice only. Phase 2 = ball + Fox only |
| Wrap-up | `bye_required_bunny` then `bye_required_fox`; no tap, no score, no fail |
| Correct | star + grass-green copy, celebration ≤1.2s |
| Wrong | shake + warm orange + retry + Listen pulses again |

**Listen sub-states:** `waiting-to-listen` → `playing` → `waiting-choice` → `correct` / `wrong`  
**Beats:** Intro → Demo_Hi → Choice_Hi → Demo_Come → Task_Invite → Optional_Rec → ByeBunny → ByeFox → Outro  
(`L1Beat` + `ListenUiState` on `HelloFriendsLevel`)

Child taps **Listen** first (does not auto-play). Hotspots unlock after the Hi demo. Ball unlocks after the Come-play demo.

Scaffold 1 = full labels + pulse, 2 = short labels, 3 = withdrawn.

## SpeakFeedback (跟读)

Panel `SpeakFeedbackPanel` (runtime-built; save prefab via **Kids English → Create SpeakFeedback Prefab** → `Assets/Prefabs/SpeakFeedback.prefab`).

Layout: demo sentence + **再听** + mic **≥96dp** + star bar + **再试** / **跳过**

**States:** `idle` → `listening` (5–8s window, auto-stop ~6.5s or tap mic) → `processing` (≤1.5s) → `success` / `retry`

`blockProgress=false` on L1: Skip / success never gates clearing the level.

Same anti-punishment rules. Event names stay `OnPlayDemo` / `OnChoice` / `OnRecordStart` / `OnRecordStop` / `OnFeedback`.

## Events

Locked payload fields. See `docs/EVENTS.md` and `docs/GAMEPLAY.md`.
