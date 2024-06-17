using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEditor;
using UnityEngine;

namespace Game
{
    [CreateAssetMenu(menuName = "Localization/Stats")]
    public class StatsLocalizationSO : SerializedScriptableObject
    {
        public const string ABILITIES_DATA_PATH = "PlayerAbilities";

        public Language language;
        public Dictionary<string, Localization> stats = new();

        [Button]
        public void AddMissingStatNames()
        {
            var assets = Resources.LoadAll(ABILITIES_DATA_PATH, typeof(AbilitySO));
            foreach (var asset in assets)
            {
                var ability = asset as AbilitySO;
                foreach (var statName in ability.StatNames)
                {
                    if (!stats.ContainsKey(statName))
                    {
                        stats.Add(statName, new Localization(statName));
                    }
                }
            }
        }

        public string GetLocalizedName(string statName, out bool valueIsInversed)
        {
            valueIsInversed = false;
            if (!stats.ContainsKey(statName)) return statName;

            Localization loc = stats[statName];
            valueIsInversed = loc.valueIsInversed;
            return language switch
            {
                Language.English => loc.english,
                Language.Russian => loc.russian,
                _ => statName,
            };
        }
    }

    [System.Serializable]
    public enum Language
    {
        English,
        Russian
    }

    [System.Serializable]
    public struct Localization
    {
        public string english;
        public string russian;
        public bool valueIsInversed;

        public Localization(string baseName)
        {
            StringBuilder sb = new();
            for (int i = 0; i < baseName.Length; i++)
            {
                if (char.IsUpper(baseName[i]) && i != 0)
                    sb.Append(' ');
                sb.Append(baseName[i]);
            }
            english = sb.ToString();
            russian = "";
            valueIsInversed = false;
        }
    }
}
