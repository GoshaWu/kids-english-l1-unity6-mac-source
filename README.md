# HelloFriends L1 — Unity 6000.6.1f1 Mac source

Use branch **`cursor/unity6-mac-ready-567e`** of the Unity project. Gameplay is frozen (hi / friend / come / play + Hi! / Come play!; Bye! required, not assessed).

## Download the source zip

Direct zip (verified, no Library/Temp/Obj/Logs):

**https://litter.catbox.moe/ip8lv9.zip**

Save as `KidsEnglishL1-Unity6000.6.1f1-source.zip`.

This host keeps the file for **72 hours** from 2026-09-19. After that, recreate it from the Unity project with `./tools/pack_source_zip.sh`.

## On the Mac

```bash
mkdir -p ~/Projects
cd ~/Projects
curl -L -o KidsEnglishL1-Unity6000.6.1f1-source.zip https://litter.catbox.moe/ip8lv9.zip
unzip KidsEnglishL1-Unity6000.6.1f1-source.zip
mv KidsEnglishL1-Unity6000.6.1f1-source KidsEnglishL1
```

`~/Projects/KidsEnglishL1` must contain `Assets/`, `Packages/`, and `ProjectSettings/`.

1. Unity Hub → Open that folder with **6000.6.1f1** (Apple Silicon + MacStandaloneSupport Mono).
2. **Kids English → First Open Setup**
3. Play `Bootstrap` or **Kids English → Open HelloFriends Scene**
4. **Kids English → Build macOS Development** or:

```bash
UNITY="/Applications/Unity/Hub/Editor/6000.6.1f1/Unity.app/Contents/MacOS/Unity"
PROJECT="$HOME/Projects/KidsEnglishL1"

"$UNITY" -batchmode -nographics \
  -projectPath "$PROJECT" \
  -buildTarget OSXUniversal \
  -executeMethod KidsEnglish.Editor.MacDevBuild.Build \
  -logFile -
```

Output: `Builds/macOS/KidsEnglishL1.app` (unsigned, Mono).
