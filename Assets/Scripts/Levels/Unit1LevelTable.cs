using KidsEnglish.Progress;

namespace KidsEnglish.Levels
{
    /// <summary>
    /// Frozen Hello Friends L1–L3 id table. This week's playable loop is L1 only.
    /// L2/L3 are reserved stubs — do not mount gameplay for them yet.
    /// </summary>
    public sealed class LevelStub
    {
        public string LevelId;
        public string Title;
        public string Status;
        public string[] ReservedHotspots;
        public string[] ReservedVocab;
    }

    public static class Unit1LevelTable
    {
        public const string Shipped = "shipped";
        public const string Stub = "stub";

        public static readonly LevelStub L1 = new LevelStub
        {
            LevelId = LevelIds.HelloFriends,
            Title = "Hello, Friends!",
            Status = Shipped,
            ReservedHotspots = new[] { "hit_bunny", "hit_tree", "hit_rock", "hit_ball", "drop_fox" },
            ReservedVocab = new[] { "hi", "bye", "come_play", "friend" }
        };

        public static readonly LevelStub L2 = new LevelStub
        {
            LevelId = LevelIds.FriendMeYou,
            Title = "Friend, Me, You",
            Status = Stub,
            ReservedHotspots = new[] { "hit_friend", "hit_me", "hit_you" },
            ReservedVocab = new[] { "friend", "me", "you" }
        };

        public static readonly LevelStub L3 = new LevelStub
        {
            LevelId = LevelIds.HappySad,
            Title = "Happy / Sad",
            Status = Stub,
            ReservedHotspots = new[] { "hit_happy", "hit_sad" },
            ReservedVocab = new[] { "happy", "sad" }
        };

        public static readonly LevelStub[] All = { L1, L2, L3 };

        public static LevelStub Find(string levelId)
        {
            foreach (var row in All)
            {
                if (row.LevelId == levelId)
                    return row;
            }

            return null;
        }
    }
}
