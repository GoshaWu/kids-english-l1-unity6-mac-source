using System;
using System.Collections.Generic;
using UnityEngine;

namespace KidsEnglish.Progress
{
    /// <summary>
    /// Local-only progress. No account, no cloud save, no child name.
    /// lastRecordMark is a stub score string, not a recording transcript.
    /// </summary>
    [Serializable]
    public sealed class ProgressData
    {
        public int schemaVersion = 2;
        public bool sfxEnabled = true;
        public bool speakAlongEnabled = true;
        public string parentPin = "0000";
        public List<LevelProgress> levels = new List<LevelProgress>();
        public IdentifyLights identify = new IdentifyLights();
        public SpeakLights speakAlong = new SpeakLights();
    }

    /// <summary>
    /// 会指认 lights. Base: hi, friend, me, you, happy, sad. Bonus: play/come, bye wave.
    /// L1 this week only lights hi, playCome, byeWave.
    /// </summary>
    [Serializable]
    public sealed class IdentifyLights
    {
        public bool hi;
        public bool friend;
        public bool me;
        public bool you;
        public bool happy;
        public bool sad;
        public bool playCome;
        public bool byeWave;

        public bool Get(string id)
        {
            switch (id)
            {
                case ParentMetrics.IdentifyHi: return hi;
                case ParentMetrics.IdentifyFriend: return friend;
                case ParentMetrics.IdentifyMe: return me;
                case ParentMetrics.IdentifyYou: return you;
                case ParentMetrics.IdentifyHappy: return happy;
                case ParentMetrics.IdentifySad: return sad;
                case ParentMetrics.IdentifyPlayCome: return playCome;
                case ParentMetrics.IdentifyByeWave: return byeWave;
                default: return false;
            }
        }

        public bool Set(string id)
        {
            switch (id)
            {
                case ParentMetrics.IdentifyHi: return Turn(ref hi);
                case ParentMetrics.IdentifyFriend: return Turn(ref friend);
                case ParentMetrics.IdentifyMe: return Turn(ref me);
                case ParentMetrics.IdentifyYou: return Turn(ref you);
                case ParentMetrics.IdentifyHappy: return Turn(ref happy);
                case ParentMetrics.IdentifySad: return Turn(ref sad);
                case ParentMetrics.IdentifyPlayCome: return Turn(ref playCome);
                case ParentMetrics.IdentifyByeWave: return Turn(ref byeWave);
                default: return false;
            }
        }

        static bool Turn(ref bool flag)
        {
            if (flag)
                return false;
            flag = true;
            return true;
        }
    }

    /// <summary>
    /// 敢跟读 lights. Hi! Bye! Optional Come play! Never require name.
    /// L1 this week only lights hi when the child actually speaks.
    /// </summary>
    [Serializable]
    public sealed class SpeakLights
    {
        public bool hi;
        public bool bye;
        public bool comePlay;

        public bool Get(string id)
        {
            switch (id)
            {
                case ParentMetrics.SpeakHi: return hi;
                case ParentMetrics.SpeakBye: return bye;
                case ParentMetrics.SpeakComePlay: return comePlay;
                default: return false;
            }
        }

        public bool Set(string id)
        {
            switch (id)
            {
                case ParentMetrics.SpeakHi: return Turn(ref hi);
                case ParentMetrics.SpeakBye: return Turn(ref bye);
                case ParentMetrics.SpeakComePlay: return Turn(ref comePlay);
                default: return false;
            }
        }

        static bool Turn(ref bool flag)
        {
            if (flag)
                return false;
            flag = true;
            return true;
        }
    }

    [Serializable]
    public sealed class LevelProgress
    {
        public string levelId;
        public bool completed;
        public string completedUtc;
        public string lastRecordMark;
        public string lastRecordRelativePath;
    }
}
