using System;
using System.IO;
using System.Reflection;
using KidsEnglish.EditorTools;
using UnityEditor;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;
using UnityEngine;

namespace KidsEnglish.Editor
{
    /// <summary>
    /// Local macOS Development Build (unsigned). Menu or
    /// <c>-executeMethod KidsEnglish.Editor.MacDevBuild.Build</c>.
    /// Uses Mono when the IL2CPP Mac standalone module is missing (typical
    /// Apple Silicon Hub install that only shows MacStandaloneSupport Mono).
    /// Does not change L1 gameplay. No Mac App Store / code-signing identity.
    /// </summary>
    public static class MacDevBuild
    {
        public const string OutputRelativePath = "Builds/macOS/KidsEnglishL1.app";
        public const string ExecuteMethod = "KidsEnglish.Editor.MacDevBuild.Build";

        static readonly string[] BuildScenes =
        {
            "Assets/Scenes/Bootstrap.unity",
            "Assets/Scenes/HelloFriends.unity"
        };

        [MenuItem("Kids English/Build macOS Development")]
        public static void Build()
        {
            try
            {
                KidsEnglishFirstOpen.Run();
                EnsureStandaloneMonoBackend();
                TrySetAppleSiliconArchitecture();

                if (EditorUserBuildSettings.activeBuildTarget != BuildTarget.StandaloneOSX)
                {
                    Debug.Log("[KidsEnglish] Switching active build target to StandaloneOSX. In batchmode prefer -buildTarget OSXUniversal so this does not force a domain reload.");
                    if (!EditorUserBuildSettings.SwitchActiveBuildTarget(NamedBuildTarget.Standalone, BuildTarget.StandaloneOSX))
                    {
                        Fail("Could not switch to StandaloneOSX. Pass -buildTarget OSXUniversal, then re-run -executeMethod " + ExecuteMethod + ".");
                        return;
                    }
                }

                PlayerSettings.useMacAppStoreValidation = false;
                EditorUserBuildSettings.development = true;
                EditorUserBuildSettings.allowDebugging = true;

                var projectRoot = Path.GetDirectoryName(Application.dataPath);
                if (string.IsNullOrEmpty(projectRoot))
                    projectRoot = Directory.GetCurrentDirectory();

                var output = Path.Combine(projectRoot, OutputRelativePath);
                var outputDir = Path.GetDirectoryName(output);
                if (!string.IsNullOrEmpty(outputDir))
                    Directory.CreateDirectory(outputDir);

                var options = new BuildPlayerOptions
                {
                    scenes = BuildScenes,
                    locationPathName = output,
                    target = BuildTarget.StandaloneOSX,
                    targetGroup = BuildTargetGroup.Standalone,
                    options = BuildOptions.Development | BuildOptions.AllowDebugging
                };

                var backend = PlayerSettings.GetScriptingBackend(NamedBuildTarget.Standalone);
                Debug.Log("[KidsEnglish] Building macOS Development (unsigned, " + backend + ") → " + output);
                var report = BuildPipeline.BuildPlayer(options);
                var summary = report.summary;
                if (summary.result != BuildResult.Succeeded)
                {
                    Fail("macOS Development Build failed: " + summary.result + " errors=" + summary.totalErrors);
                    return;
                }

                Debug.Log("[KidsEnglish] macOS Development Build succeeded: " + output + " (local Dev, no code signing, scripting backend " + backend + ").");
                ExitBatch(0);
            }
            catch (Exception ex)
            {
                Debug.LogError("[KidsEnglish] macOS Development Build exception: " + ex);
                ExitBatch(1);
                throw;
            }
        }

        /// <summary>
        /// Force Standalone to Mono when IL2CPP playback files are absent.
        /// The Mac Dev menu also prefers Mono so a Mono-only Hub module can build.
        /// </summary>
        public static void EnsureStandaloneMonoBackend()
        {
            var named = NamedBuildTarget.Standalone;
            var current = PlayerSettings.GetScriptingBackend(named);
            var il2cppPresent = LooksLikeStandaloneIl2CppInstalled();

            if (current == ScriptingImplementation.Mono2x)
            {
                Debug.Log("[KidsEnglish] Standalone scripting backend is Mono.");
                return;
            }

            if (il2cppPresent && current == ScriptingImplementation.IL2CPP)
            {
                PlayerSettings.SetScriptingBackend(named, ScriptingImplementation.Mono2x);
                Debug.Log("[KidsEnglish] Standalone scripting backend was IL2CPP; Mac Dev Build switches to Mono (IL2CPP is not required).");
                return;
            }

            PlayerSettings.SetScriptingBackend(named, ScriptingImplementation.Mono2x);
            if (!il2cppPresent)
                Debug.Log("[KidsEnglish] IL2CPP Mac standalone module not found. Standalone scripting backend set to Mono.");
            else
                Debug.Log("[KidsEnglish] Standalone scripting backend set to Mono for macOS Development Build.");
        }

        static bool LooksLikeStandaloneIl2CppInstalled()
        {
            try
            {
                var engineDir = BuildPipeline.GetPlaybackEngineDirectory(BuildTarget.StandaloneOSX, BuildOptions.None);
                if (string.IsNullOrEmpty(engineDir) || !Directory.Exists(engineDir))
                    return false;

                var needle = "il2cpp";
                if (engineDir.IndexOf(needle, StringComparison.OrdinalIgnoreCase) >= 0)
                    return true;

                foreach (var entry in Directory.EnumerateFileSystemEntries(engineDir))
                {
                    if (Path.GetFileName(entry).IndexOf(needle, StringComparison.OrdinalIgnoreCase) >= 0)
                        return true;
                }

                var variations = Path.Combine(engineDir, "Variations");
                if (!Directory.Exists(variations))
                    return false;

                foreach (var dir in Directory.GetDirectories(variations))
                {
                    if (Path.GetFileName(dir).IndexOf(needle, StringComparison.OrdinalIgnoreCase) >= 0)
                        return true;
                }
            }
            catch (Exception ex)
            {
                Debug.Log("[KidsEnglish] Could not probe IL2CPP module (" + ex.Message + "). Assuming it is missing and using Mono.");
            }

            return false;
        }

        static void TrySetAppleSiliconArchitecture()
        {
            var type = Type.GetType("UnityEditor.OSXStandalone.UserBuildSettings,UnityEditor.OSXStandalone.Extensions")
                       ?? Type.GetType("UnityEditor.OSXStandalone.UserBuildSettings,UnityEditor");
            if (type == null)
                return;

            var prop = type.GetProperty("architecture", BindingFlags.Public | BindingFlags.Static);
            if (prop == null || !prop.CanWrite)
                return;

            var enumType = prop.PropertyType;
            object value = null;
            foreach (var name in new[] { "ARM64", "AppleSilicon", "AppleSilicon64" })
            {
                try
                {
                    value = Enum.Parse(enumType, name, true);
                    break;
                }
                catch (ArgumentException)
                {
                }
            }

            if (value == null && enumType.IsEnum)
                value = Enum.ToObject(enumType, 1);

            if (value == null)
                return;

            prop.SetValue(null, value);
            Debug.Log("[KidsEnglish] macOS architecture set to Apple silicon (" + value + ").");
        }

        static void Fail(string message)
        {
            Debug.LogError("[KidsEnglish] " + message);
            ExitBatch(1);
        }

        static void ExitBatch(int code)
        {
            if (!Application.isBatchMode)
                return;
            EditorApplication.Exit(code);
        }
    }
}
