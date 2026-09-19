# Hello Friends levels (content freeze)

**L1–L3 scripts are content-frozen.** The two-week prototype **acceptance is still L1 only** (one-level HelloFriends loop, including `bye_required_bunny` / `bye_required_fox`).

Do not implement L2 or L3 gameplay this week. Do not mount Friend/Me/You or Happy/Sad into the HelloFriends scene.

| Level id | Status | Content (frozen) |
| --- | --- | --- |
| `L1_HelloFriends` | **shipped this week** | Bunny / Tree / Rock → Hi; ball → Fox Come play; optional Hi; Bye Bunny then Bye Fox |
| `L2_FriendMeYou` | stub only | Reserved hotspots `hit_friend` / `hit_me` / `hit_you` |
| `L3_HappySad` | stub only | Reserved hotspots `hit_happy` / `hit_sad` |

C# table: `KidsEnglish.Levels.Unit1LevelTable`  
JSON table: `Assets/Addressables/Levels/unit1.levels.json`  
Empty hooks: `Assets/Addressables/Levels/L2_FriendMeYou/`, `L3_HappySad/`, `UnitVocab/`

Ids: `KidsEnglish.Progress.LevelIds`.

## L1 acceptance (this week)

1. Bunny / Tree / Rock → tap Bunny for “Hi”
2. Ball → Fox for “Come play!”
3. Optional speak-along “Hi” (`blockProgress=false`)
4. `bye_required_bunny` then `bye_required_fox` (wave + Bye!, `assess=false`, does not block clear)

Keys **forbidden** as HelloFriends tasks: `me`, `you`, `happy`, `sad`, `name`.

## Later layouts (not this week)

| Layout | Typical keys |
| --- | --- |
| Hi / Bye | `hi`, `bye` |
| Happy / Sad | `happy`, `sad` |
| Play 2×2 | `play` + three distractors |
| Friend / Me / You | `friend`, `me`, `you` |
