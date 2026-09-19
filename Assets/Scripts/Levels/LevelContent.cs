using UnityEngine;

namespace KidsEnglish.Levels
{
    [System.Serializable]
    public sealed class ChoiceOption
    {
        public string id;
        public string labelEn;
        public string labelZh;
        public bool isCorrect;
        public string spriteKey;
        public string resourcesSprite;
        public Color tint = Color.white;
    }

    /// <summary>
    /// Authoring stub only. HelloFriendsLevel does not read this asset — L1 v1
    /// beats are hardcoded. Keep defaults on Bunny/Tree/Rock; do not fill with
    /// me / you / happy / sad / name.
    /// </summary>
    [CreateAssetMenu(menuName = "Kids English/Level Content", fileName = "LevelContent")]
    public sealed class LevelContent : ScriptableObject
    {
        public string levelId = "L1_HelloFriends";
        public string title = "Hello, Friends!";
        public string subtitle = "Listen, then tap what you heard  听一听，点一点";
        public string demoKey = Core.ContentKeys.Audio(Core.ContentKeys.Hi);
        public string demoResourcesPath = Core.ContentKeys.Audio(Core.ContentKeys.Hi);
        public string promptPhrase = Core.ContentKeys.Hi;
        public ChoiceOption[] choices =
        {
            new ChoiceOption { id = L1HelloFriends.Bunny, labelEn = "Bunny", labelZh = "兔子", isCorrect = true, spriteKey = L1HelloFriends.CharBunny },
            new ChoiceOption { id = L1HelloFriends.Tree, labelEn = "Tree", labelZh = "树", isCorrect = false, spriteKey = L1HelloFriends.PropTree },
            new ChoiceOption { id = L1HelloFriends.Rock, labelEn = "Rock", labelZh = "石头", isCorrect = false, spriteKey = L1HelloFriends.PropRock }
        };
    }
}
