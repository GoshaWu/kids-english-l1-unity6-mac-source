# Addressables data (generated in Editor)

Unity Addressables writes binary/YAML settings here the first time you open
**Window → Asset Management → Addressables → Groups** or run
**Kids English → First Open Setup**.

This Cloud environment never launched Unity Editor, so checked-in files are
the grouping *convention* only (`groups.convention.json`). After First Open
Setup you should see groups:

- `Audio_Vocab` / `Audio_Pattern` — addresses `Audio/{key}`
- `Art_Vocab` / `Art_Pattern` — addresses `Art/{key}`
- `Art_Scene` — park-gate + L1 anim chips (`Art/{id}`)
- `Audio_L1` — `vo_who_says_hi`, `vo_come_play_hint`, `vo_hi_speak`, `vo_bye_expose`
- `Audio_Sfx` — `sfx_star`, `sfx_wrong_soft` (+ fallbacks)
- `Levels`

L1 P0 ids are exact; see `docs/ASSETS.md`. Unit catalog keys may coexist; HelloFriends does not run the 8-card quiz.

Locked Unit 1 keys (no aliases): hi, bye, friend, me, you, happy, sad, play,
name, come, hi_bye, come_play.
