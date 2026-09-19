using KidsEnglish.Core;
using KidsEnglish.Editor;
using KidsEnglish.Levels;
using UnityEditor;
using UnityEditor.AddressableAssets;
using UnityEditor.AddressableAssets.Settings;
using UnityEditor.AddressableAssets.Settings.GroupSchemas;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace KidsEnglish.EditorTools
{
    /// <summary>
    /// Run once after cloning: Addressables groups, Unit 1 key slots, build scenes, URP assignment, Mono backend.
    /// This Cloud environment did not run Unity Editor; humans should run this menu locally.
    /// </summary>
    public static class KidsEnglishFirstOpen
    {
        const string AudioVocabGroup = "Audio_Vocab";
        const string AudioPatternGroup = "Audio_Pattern";
        const string ArtVocabGroup = "Art_Vocab";
        const string ArtPatternGroup = "Art_Pattern";
        const string AudioSfxGroup = "Audio_Sfx";
        const string ArtSceneGroup = "Art_Scene";
        const string AudioL1Group = "Audio_L1";
        const string LevelsGroup = "Levels";

        [MenuItem("Kids English/First Open Setup")]
        public static void Run()
        {
            EnsureBuildScenes();
            EnsureUrpAssigned();
            MacDevBuild.EnsureStandaloneMonoBackend();
            var settings = EnsureAddressables();

            foreach (var key in ContentKeys.Vocab)
            {
                Mark(settings, ContentKeys.AudioAssetPath(key), ContentKeys.Audio(key), AudioVocabGroup, key, "vocab");
                Mark(settings, ContentKeys.ArtAssetPath(key), ContentKeys.Art(key), ArtVocabGroup, key, "vocab");
            }

            foreach (var key in ContentKeys.Patterns)
            {
                Mark(settings, ContentKeys.AudioAssetPath(key), ContentKeys.Audio(key), AudioPatternGroup, key, "pattern");
                Mark(settings, ContentKeys.ArtAssetPath(key), ContentKeys.Art(key), ArtPatternGroup, key, "pattern");
            }

            foreach (var key in L1HelloFriends.ParkGateArt)
                Mark(settings, ContentKeys.ArtAssetPath(key), ContentKeys.Art(key), ArtSceneGroup, key, "scene");

            foreach (var key in L1HelloFriends.L1Anims)
                Mark(settings, ContentKeys.ArtAssetPath(key), ContentKeys.Art(key), ArtSceneGroup, key, "scene");

            foreach (var key in L1HelloFriends.L1Vo)
                Mark(settings, ContentKeys.AudioAssetPath(key), ContentKeys.Audio(key), AudioL1Group, key, "vo");

            foreach (var key in L1HelloFriends.L1Sfx)
                Mark(settings, ContentKeys.SfxAssetPath(key), key, AudioSfxGroup, key, "sfx");

            Mark(settings, ContentKeys.SfxAssetPath(ContentKeys.SfxSuccess), ContentKeys.SfxSuccess, AudioSfxGroup, ContentKeys.SfxSuccess, "sfx");
            Mark(settings, ContentKeys.SfxAssetPath(ContentKeys.SfxRetry), ContentKeys.SfxRetry, AudioSfxGroup, ContentKeys.SfxRetry, "sfx");

            AssetDatabase.SaveAssets();
            Debug.Log("[KidsEnglish] First Open Setup finished. L1 P0 Art ids marked (Art_Scene + Audio_L1). Unit 1 card_* slots reserved; HelloFriends does not run the 8-card quiz.");
        }

        static void EnsureBuildScenes()
        {
            var bootstrap = "Assets/Scenes/Bootstrap.unity";
            var hello = "Assets/Scenes/HelloFriends.unity";
            EditorBuildSettings.scenes = new[]
            {
                new EditorBuildSettingsScene(bootstrap, true),
                new EditorBuildSettingsScene(hello, true)
            };
        }

        static void EnsureUrpAssigned()
        {
            var pipeline = AssetDatabase.LoadAssetAtPath<UniversalRenderPipelineAsset>("Assets/Settings/URP-2D-Pipeline.asset");
            if (pipeline == null)
            {
                Debug.LogWarning("[KidsEnglish] Missing Assets/Settings/URP-2D-Pipeline.asset. Create URP Asset (with 2D Renderer) if the pink-scene warning appears.");
                return;
            }

            GraphicsSettings.defaultRenderPipeline = pipeline;
            QualitySettings.renderPipeline = pipeline;
            Debug.Log("[KidsEnglish] Assigned URP-2D-Pipeline to Graphics/Quality settings.");
        }

        static AddressableAssetSettings EnsureAddressables()
        {
            var settings = AddressableAssetSettingsDefaultObject.GetSettings(true);
            EnsureGroup(settings, AudioVocabGroup);
            EnsureGroup(settings, AudioPatternGroup);
            EnsureGroup(settings, ArtVocabGroup);
            EnsureGroup(settings, ArtPatternGroup);
            EnsureGroup(settings, ArtSceneGroup);
            EnsureGroup(settings, AudioL1Group);
            EnsureGroup(settings, AudioSfxGroup);
            EnsureGroup(settings, LevelsGroup);
            return settings;
        }

        static AddressableAssetGroup EnsureGroup(AddressableAssetSettings settings, string name)
        {
            var group = settings.FindGroup(name);
            if (group != null)
                return group;
            return settings.CreateGroup(name, false, false, true, null, typeof(ContentUpdateGroupSchema), typeof(BundledAssetGroupSchema));
        }

        static void Mark(AddressableAssetSettings settings, string assetPath, string address, string groupName, string contentKey, string kind)
        {
            var guid = AssetDatabase.AssetPathToGUID(assetPath);
            if (string.IsNullOrEmpty(guid))
            {
                Debug.LogWarning("[KidsEnglish] Skip missing asset: " + assetPath);
                return;
            }

            var group = EnsureGroup(settings, groupName);
            var entry = settings.CreateOrMoveEntry(guid, group, false, false);
            entry.SetAddress(address);
            entry.SetLabel("kids-english", true, true);
            entry.SetLabel("unit1", true, true);
            entry.SetLabel(kind, true, true);
            entry.SetLabel(contentKey, true, true);
        }

        [MenuItem("Kids English/Open HelloFriends Scene")]
        public static void OpenHelloFriends()
        {
            EditorSceneManagement.OpenScene("Assets/Scenes/HelloFriends.unity");
        }

        [MenuItem("Kids English/Create SpeakFeedback Prefab")]
        public static void CreateSpeakFeedbackPrefab()
        {
            System.IO.Directory.CreateDirectory("Assets/Prefabs");
            var go = new GameObject("SpeakFeedback", typeof(RectTransform), typeof(SpeakFeedbackPanel));
            var path = "Assets/Prefabs/SpeakFeedback.prefab";
            PrefabUtility.SaveAsPrefabAsset(go, path);
            Object.DestroyImmediate(go);
            Debug.Log("[KidsEnglish] Wrote " + path + " (runtime Build() fills the wireframe on instantiate).");
        }
    }
}
