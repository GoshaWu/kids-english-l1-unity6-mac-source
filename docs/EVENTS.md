# Event contract — L1_HelloFriends

玩法 / 美术 / 音频 listen on `KidsEnglish.Core.GameEventBus` **or** instance events on `KidsEnglish.Levels.LevelController`. **Event names are exact.**

Payload fields below are the locked script names (`clipId`, `choiceId`, …). C# uses PascalCase (`ClipId`, `ChoiceId`, …). `LevelId` is extra routing (`L1_HelloFriends`).

No payload includes a child name, account id, face, or raw PCM bytes.

## `OnPlayDemo`

`{ clipId, targetId?, canReplay, assess }`

| Field | Type | Meaning |
| --- | --- | --- |
| `clipId` | string | `hi`, `come_play`, `bye_required_bunny`, `bye_required_fox` |
| `targetId` | string? | `bunny` (Hi / first Bye), `fox` (Come play / second Bye) |
| `canReplay` | bool | Listen stays available after the clip (`false` on end-of-level Bye) |
| `assess` | bool | `false` for listen-only (Hi demo, Come-play demo, both Bye demos). Assessed work is `OnChoice` / record, never Bye. |

**音频:** `AudioPlayer` is already playing; do not start a second voice. A choice or record **stops** the demo.

End-of-level Bye is two required listen-only demos (`assess=false`), **not** success criteria, and **does not** block clear (progress is saved after the invite). **Event `clipId` stays `bye_required_bunny` / `bye_required_fox`.** Audio bytes come from `Audio/vo_bye_expose` (fallback `Audio/bye`):

1. `bye_required_bunny` — Bunny waves (`anim_wave_bunny`) + “Bye!” (`targetId=bunny`)
2. `bye_required_fox` — Fox waves (`anim_fox_wave`) + “Bye!” (`targetId=fox`)

No tap choice, no scoring, no fail path. There is no `bye_optional` id.

## `OnChoice`

`{ choiceId, optionId, correct }`

Tap and **drag-drop use this same shape**.

| Field | Type | Meaning |
| --- | --- | --- |
| `choiceId` | string | Task: `hi` or `come_play` |
| `optionId` | string | Hi: `bunny` / `tree` / `rock`. Invite: `fox` only (phase 2 drop). Missed drops bounce and do not fire `OnChoice`. |
| `correct` | bool | Bunny is correct for Hi; Fox is correct for Come play |

Missed ball drops (empty space) do **not** fire `OnChoice`. Wrong taps/drops are **not** a score-down.

## `OnRecordStart`

`{ utteranceId }`

| Field | Type | Meaning |
| --- | --- | --- |
| `utteranceId` | string | Always `"hi"` on this level |

## `OnRecordStop`

`{ utteranceId, audioRef?, score? }`

| Field | Type | Meaning |
| --- | --- | --- |
| `utteranceId` | string | `"hi"` |
| `audioRef` | string? | Local wav path under `voice_local/` (may be empty) |
| `score` | float? | Mocked stub (no cloud ASR) |

## `OnFeedback`

`{ kind, textKey, scaffoldLevel? }`

| Field | Type | Meaning |
| --- | --- | --- |
| `kind` | string | `intro` `demo` `success` `retry` `skip` `record_stub` `outro` |
| `textKey` | string | Stable id, e.g. `l1.hi.success` |
| `scaffoldLevel` | int? | `1` full · `2` reduced · `3` withdrawn |

| kind | Suggested art |
| --- | --- |
| `success` | Star + grass green |
| `retry` | Shake + warm orange — never a red X |
| `skip` | Wave, no punishment (`blockProgress=false`) |
| `record_stub` | Smile + “Nice speaking!” |
| `outro` | Short celebration ≤1.2s |

## Wiring

```csharp
GameEventBus.OnPlayDemo += payload => { /* payload.ClipId */ };
GetComponent<LevelController>().OnChoice += payload => { };
```

Call `GameEventBus.ClearAll()` only when tearing down play mode.

Hi listen-only demo plays `vo_who_says_hi` (event `clipId=hi`). Come-play demo plays `vo_come_play_hint` (event `clipId=come_play`). SpeakFeedback optional Hi! uses `vo_hi_speak`. Success / miss SFX: `sfx_star` / `sfx_wrong_soft`. Full P0 list: [ASSETS.md](ASSETS.md).
