using KidsEnglish.Core;
using UnityEngine;

namespace KidsEnglish.Levels
{
    /// <summary>
    /// Unit 1 reserved key catalog. Create via Assets → Create → Kids English → Unit 1 Catalog.
    /// Keys are locked; this asset is a slot list for 玩法 / 美术, not a spelling source.
    /// </summary>
    [CreateAssetMenu(menuName = "Kids English/Unit 1 Catalog", fileName = "Unit1Catalog")]
    public sealed class Unit1Catalog : ScriptableObject
    {
        public string unitId = "unit1";
        public string[] vocabKeys;
        public string[] patternKeys;

        void Reset()
        {
            unitId = "unit1";
            vocabKeys = (string[])ContentKeys.Vocab.Clone();
            patternKeys = (string[])ContentKeys.Patterns.Clone();
        }

        public static Unit1Catalog BuiltIn()
        {
            var so = CreateInstance<Unit1Catalog>();
            so.Reset();
            return so;
        }
    }
}
