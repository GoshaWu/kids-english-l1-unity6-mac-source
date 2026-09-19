using System;
using System.IO;
using UnityEngine;

namespace KidsEnglish.Progress
{
    /// <summary>
    /// JSON store under persistentDataPath. Prefer this over PlayerPrefs so parents can find/delete the file.
    /// Child data minimization: level id, completed flag, 会指认 / 敢跟读 lights. No accuracy, no ranking.
    /// </summary>
    public sealed class ProgressStore : MonoBehaviour
    {
        public const string FileName = "kids_english_progress.json";

        public ProgressData Data { get; private set; } = new ProgressData();

        public string FilePath => Path.Combine(Application.persistentDataPath, FileName);

        public void Load()
        {
            try
            {
                if (File.Exists(FilePath))
                {
                    var json = File.ReadAllText(FilePath);
                    Data = JsonUtility.FromJson<ProgressData>(json) ?? new ProgressData();
                    if (Data.levels == null)
                        Data.levels = new System.Collections.Generic.List<LevelProgress>();
                    EnsureMetrics();
                }
                else
                {
                    Data = new ProgressData();
                    EnsureMetrics();
                    Save();
                }
            }
            catch (Exception ex)
            {
                Debug.LogWarning($"[KidsEnglish] Progress load failed, starting fresh: {ex.Message}");
                Data = new ProgressData();
                EnsureMetrics();
            }
        }

        public void Save()
        {
            try
            {
                var json = JsonUtility.ToJson(Data, true);
                File.WriteAllText(FilePath, json);
            }
            catch (Exception ex)
            {
                Debug.LogWarning($"[KidsEnglish] Progress save failed: {ex.Message}");
            }
        }

        public LevelProgress GetLevel(string levelId)
        {
            foreach (var level in Data.levels)
            {
                if (level.levelId == levelId)
                    return level;
            }

            var created = new LevelProgress { levelId = levelId };
            Data.levels.Add(created);
            return created;
        }

        public void MarkHelloFriendsComplete(string lastRecordMark, string lastRecordRelativePath)
        {
            var level = GetLevel(LevelIds.HelloFriends);
            level.completed = true;
            level.completedUtc = DateTime.UtcNow.ToString("o");
            if (!string.IsNullOrEmpty(lastRecordMark))
                level.lastRecordMark = lastRecordMark;
            if (!string.IsNullOrEmpty(lastRecordRelativePath))
                level.lastRecordRelativePath = lastRecordRelativePath;
            Save();
        }

        public void SetLastRecord(string lastRecordMark, string lastRecordRelativePath)
        {
            var level = GetLevel(LevelIds.HelloFriends);
            level.lastRecordMark = lastRecordMark ?? string.Empty;
            level.lastRecordRelativePath = lastRecordRelativePath ?? string.Empty;
            Save();
        }

        public void SetSfx(bool enabled)
        {
            Data.sfxEnabled = enabled;
            Save();
        }

        public void SetSpeakAlong(bool enabled)
        {
            Data.speakAlongEnabled = enabled;
            Save();
        }

        public void LightIdentify(string id)
        {
            EnsureMetrics();
            if (Data.identify.Set(id))
                Save();
        }

        public void LightSpeak(string id)
        {
            EnsureMetrics();
            if (Data.speakAlong.Set(id))
                Save();
        }

        void EnsureMetrics()
        {
            if (Data == null)
                Data = new ProgressData();
            if (Data.identify == null)
                Data.identify = new IdentifyLights();
            if (Data.speakAlong == null)
                Data.speakAlong = new SpeakLights();
            if (Data.schemaVersion < 2)
                Data.schemaVersion = 2;
        }
    }

    public static class LevelIds
    {
        public const string HelloFriends = "L1_HelloFriends";
        public const string FriendMeYou = "L2_FriendMeYou";
        public const string HappySad = "L3_HappySad";
    }
}
