# Official L1 P0 asset list (Art freeze)

Exact ids from Art. **Do not alias, rename, or invent extras** for the HelloFriends main path. Colored blocks + emoji are the checked-in placeholders until production cuts land.

Unit `card_*` / vocab library files **may coexist** in this repo. The L1 main path does **not** use the 8-card quiz string.

C# constants: `KidsEnglish.Levels.L1HelloFriends` (scene, hits, anims, VO, UI) and `KidsEnglish.Core.ContentKeys` (unit catalog + Addressable path helpers).

## Scene & characters

| Id | Role | File |
| --- | --- | --- |
| `bg_park_gate` | Park-gate backdrop | `Assets/Art/bg_park_gate.png` |
| `char_bunny` | Guide; wave / talk simple frames | `Assets/Art/char_bunny.png` |
| `char_fox` | New friend; catch-happy, wave | `Assets/Art/char_fox.png` |
| `prop_tree` | Hi distractor | `Assets/Art/prop_tree.png` |
| `prop_rock` | Hi distractor | `Assets/Art/prop_rock.png` |
| `prop_ball` | Invite prop. Visual **~56**; hit is `hit_ball` **80×80** | `Assets/Art/prop_ball.png` |

Address: `Art/{id}`. Group: `Art_Scene`. Runtime GameObject for the stage is named `bg_park_gate`; character portrait children use the `char_*` / `prop_*` ids.

`drop_fox.png` is the Fox drop-zone chip (same size as the hotspot). It is **not** a sixth Hi choice.

## Hotspots (390×844 pt, edge gap ≥24, phase lock ① then ②)

Hit boxes are runtime rects. GameObject **name is the locked hit id**. Do not add `hit_friend`, `hit_me`, or other L2/L3 hits to this scene.

| Id | Size | Phase | Role |
| --- | --- | --- | --- |
| `hit_bunny` | **96×140** | ① three-choice | Tap Bunny for Hi |
| `hit_tree` | **88×120** | ① three-choice | Distractor |
| `hit_rock` | **88×88** | ① three-choice | Distractor |
| `hit_ball` | **80×80** | ② ball + Fox | Drag or tap-to-send |
| `drop_fox` | **100×120**, snap **64** | ② ball + Fox | Fox catch target |

Phase 1 enables **only** Bunny / Tree / Rock. Phase 2 enables **only** `hit_ball` + `drop_fox` (three-choice stays as scenery). Wrap-up: **no tap, no score**.

## Anims / events

Placeholder chips in `Assets/Art/` (simple colored frames until cuts). Runtime also plays `UiMotion.Wave` / `UiMotion.Catch`.

| Id | When |
| --- | --- |
| `anim_bunny_wave` | Intro Bunny wave |
| `anim_wave_bunny` | `bye_required_bunny` exposure |
| `anim_fox_wave` | `bye_required_fox` exposure |
| `anim_fox_catch` | Ball lands on Fox |

**Event clipIds** (OnPlayDemo — do **not** rename these to `vo_*`):

| clipId | assess | Audio file |
| --- | --- | --- |
| `hi` | `false` (listen-only demo) | `vo_who_says_hi` (fallback `hi.wav`) |
| `come_play` | `false` (listen-only demo) | `vo_come_play_hint` (fallback `come_play.wav`) |
| `bye_required_bunny` | `false`, exposure only | `vo_bye_expose` (fallback `bye.wav`) |
| `bye_required_fox` | `false`, exposure only | `vo_bye_expose` (fallback `bye.wav`) |

There is no `bye_optional` id. Wrap-up never scores and never blocks clear.

## UI reuse

| Id | Role |
| --- | --- |
| `ui_btn_listen` | Listen demo button (≥80dp + slop) |
| `SpeakFeedback` | Optional Hi! (`blockProgress=false`); retry / skip |
| `ui_parent_lock` | Parent 🔒 long-press |
| `progress_dots` | Four-dot beat progress |

## Audio stubs

| Id | Use | Path |
| --- | --- | --- |
| `vo_who_says_hi` | Who says Hi? demo bytes | `Assets/Audio/vo_who_says_hi.wav` |
| `vo_come_play_hint` | Come play hint bytes | `Assets/Audio/vo_come_play_hint.wav` |
| `vo_hi_speak` | SpeakFeedback Hi! prompt | `Assets/Audio/vo_hi_speak.wav` |
| `vo_bye_expose` | Both required Bye exposures | `Assets/Audio/vo_bye_expose.wav` |
| `sfx_star` | Success star | `Assets/Audio/Sfx/sfx_star.wav` |
| `sfx_wrong_soft` | Soft miss (no red X) | `Assets/Audio/Sfx/sfx_wrong_soft.wav` |

VO addresses: `Audio/{id}` in group `Audio_L1`. SFX addresses: exact id (`sfx_star`) in group `Audio_Sfx`. Legacy `sfx_success` / `sfx_retry` remain as fallbacks only.

Regenerate placeholders: `python3 tools/make_l1_p0_placeholders.py` (anims + VO + sfx). Park-gate characters: `python3 tools/make_park_placeholders.py`.

## Unit vocab library (not the L1 quiz)

These keys stay reserved for later levels. **HelloFriends must not present them as a card quiz.**

Vocabulary: `hi` · `bye` · `friend` · `me` · `you` · `happy` · `sad` · `play` · `name` · `come`

Patterns: `hi_bye` · `come_play`

Forbidden as HelloFriends **tasks**: `me`, `you`, `happy`, `sad`, `name` (`L1HelloFriends.ForbiddenInThisLevel`).

Canonical files (overwrite; keep the filename):

```
Assets/Audio/{key}.wav
Assets/Art/{key}.png
```

Addresses `Audio/{key}` / `Art/{key}`. Groups `Audio_Vocab` / `Art_Vocab` / `Audio_Pattern` / `Art_Pattern`.

## Addressable groups

Create via **Kids English → First Open Setup**.

| Group | Addresses |
| --- | --- |
| `Art_Scene` | `Art/bg_park_gate` … `Art/prop_ball`, `Art/drop_fox`, `Art/anim_*` |
| `Audio_L1` | `Audio/vo_who_says_hi`, `Audio/vo_come_play_hint`, `Audio/vo_hi_speak`, `Audio/vo_bye_expose` |
| `Audio_Sfx` | `sfx_star`, `sfx_wrong_soft` (+ fallback `sfx_success`, `sfx_retry`) |
| `Audio_Vocab` / `Art_Vocab` | unit catalog — **not** mounted in HelloFriends |
| `Audio_Pattern` / `Art_Pattern` | `hi_bye`, `come_play` catalog slots |
| `Levels` | `L1_HelloFriends` (L2/L3 stubs only) |

Labels: `kids-english`, `unit1`, `scene`|`vo`|`sfx`|`vocab`|`pattern`, plus the exact key.

Convention file: `Assets/AddressableAssetsData/groups.convention.json`.

`ContentLoader` order: Addressables → `Resources/` copies → generated beep / colored circle.

## Art spec (placeholders)

- Canvas reference **390×844**; 1 unit = 1 logical px
- Character chips: high-contrast silhouettes; Tree/Rock stay friendly (no “wrong” face)
- `prop_ball` visual ~56 inside an 80×80 hit
- Word is a UI label; art does not have to letter the English
- Production cuts overwrite the same filenames

## Audio spec

- One speaker, slow, smiling. Peak around -6 dBFS. No music under the line.
- Speak-along wavs are **not** Addressables (`persistentDataPath/voice_local/`, never uploaded).

## How to drop production content

1. Overwrite `Assets/Art/{exact id}.png` and `Assets/Audio/{exact id}.wav` (and `Assets/Audio/Sfx/{sfx id}.wav`).
2. Run **Kids English → First Open Setup**.
3. Play `HelloFriends`. Do not add new hotspot ids to this scene.
