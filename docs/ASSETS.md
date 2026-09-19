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
