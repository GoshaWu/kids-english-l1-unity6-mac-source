# Hello, Friends! — Kids English (Unity 2D)

Clone this repo, then open the folder that contains `Assets/`, `Packages/`, and `ProjectSettings/` in **Unity 6000.6.1f1**.

```bash
gh repo clone GoshaWu/kids-english-l1-unity6-mac-source ~/Projects/KidsEnglishL1
```

`ProjectSettings/ProjectVersion.txt` is **6000.6.1f1** (`7efac9f6c10e`). L1 gameplay is frozen. See [docs/MAC_UNITY6.md](docs/MAC_UNITY6.md).

---

引导式英语启蒙 prototype for ages **4–6**. First playable slice is **L1_HelloFriends**: listen and tap Bunny for “Hi”, drag the ball to Fox for “Come play!”, then optional speak-along. Not a quiz. Speak-along is a **local mic + scoring stub** only (`blockProgress=false`).

This repository is **Unity 6 ready**. Play Mode and the macOS `.app` were **not** run in the Linux cloud VM (no Unity Editor there). Open it on a Mac with **6000.6.1f1** and follow [docs/MAC_UNITY6.md](docs/MAC_UNITY6.md).

## Unity version

| | After |
| --- | --- |
| Editor | **Unity 6** `6000.6.1f1` (`7efac9f6c10e`) |
| URP 2D | **17.6.0** |
| Addressables | **2.11.2** |
| UI | **uGUI 2.0.0** |
| Scripting backend (Standalone) | **Mono** |

## Open / Play / Build

1. Hub → Open this folder with **6000.6.1f1** (Apple Silicon + MacStandaloneSupport Mono).
2. **Kids English → First Open Setup**
3. Play `Bootstrap` or **Kids English → Open HelloFriends Scene**
4. **Kids English → Build macOS Development** or `-executeMethod KidsEnglish.Editor.MacDevBuild.Build`

Output: `Builds/macOS/KidsEnglishL1.app` (unsigned, Mono).
