using System;
using UnityEngine;
using System.Collections.Generic;
using Sirenix.OdinInspector;

namespace Game
{
    [CreateAssetMenu(menuName = "Ability")]
    public class AbilitySO : ScriptableObject
    {
        public static readonly string[] BaseStats = new[] { 
            "Quantity", "Size", "Power", "Duration", "Cooldown" };

        [Title("Data")]
        [PreviewField] 
        public Sprite icon;
        public string abilityName = "N/A";
        [Multiline]
        public string quickDescription = "N/A";
        [Multiline]
        public string fullDescription = "N/A";

        [Title("Base Stats")]
        [SerializeField] private float _quantity = 1;
        [SerializeField] private float _size = 1;
        [SerializeField] private float _power = 1;
        [SerializeField] private float _duration = 3;
        [SerializeField] private float _cooldown = 1;

        [Title("Custom Stats")]
        public List<StatInfo> statInfos = new();

        [PropertyOrder(3), Title("Upgrades")]
        public List<UpgradeInfo> upgradeInfos = new();

        public Dictionary<string, Stat> StatsDictionary = new();
        public Stat Quantity { get => baseStatInfos[0].stat; }
        public Stat Size { get => baseStatInfos[1].stat; }
        public Stat Power { get => baseStatInfos[2].stat; }
        public Stat Duration { get => baseStatInfos[3].stat; }
        public Stat Cooldown { get => baseStatInfos[4].stat; }

        [HideInInspector]
        public List<StatInfo> baseStatInfos = CreateStatInfosFromNames(BaseStats);
        public string[] StatNames { get; protected set; }
        public int UpgradeLevel { get => _upgradeLevel; }
        private bool _isLevelApplied = false;

        [BoxGroup("Mods", GroupName = "Modified Values", VisibleIf = "_isLevelApplied")]
        [PropertyOrder(1)]
        [SerializeField, ReadOnly] private int _upgradeLevel;

        [BoxGroup("Mods")]
        [InfoBox("$GetStatValuesFormated", "_isLevelApplied")]
        [Button(size: ButtonSizes.Medium), PropertyOrder(2)]
        private void ClearAppliedModifiers()
        {
            InitializeStatReferences();
            foreach (var statName in StatsDictionary.Keys)
            {
                StatsDictionary[statName].RemoveAllModifiers();
            }
            foreach (var upgradeInfo in upgradeInfos)
            {
                upgradeInfo.stateId = 0;
                upgradeInfo.applyUpgrades = false;
            }
            _upgradeLevel = 0;
            _isLevelApplied = false;
        }

        private void InitializeStatReferences()
        {
            baseStatInfos[0].baseValue = _quantity;
            baseStatInfos[1].baseValue = _size;
            baseStatInfos[2].baseValue = _power;
            baseStatInfos[3].baseValue = _duration;
            baseStatInfos[4].baseValue = _cooldown;

            var allStatInfos = GetAllStatInfos();
            StatNames = new string[allStatInfos.Count];
            StatsDictionary.Clear();
            for (int i = 0; i < allStatInfos.Count; i++)
            {
                var statInfo = allStatInfos[i];
                StatNames[i] = statInfo.key;
                statInfo.stat.BaseValue = statInfo.baseValue;
                if (!StatsDictionary.ContainsKey(statInfo.key))
                {
                    StatsDictionary.Add(statInfo.key, statInfo.stat);
                }
                else
                {
                    Debug.LogError("A stat with the same name has already been created. Stat name: " + statInfo.key);
                }
            }
        }

        private void OnValidate()
        {
            InitializeStatReferences();

            int appliedLevel = -1;
            for (int i = upgradeInfos.Count - 1; i >= 0; i--)
            {
                var upgradeInfo = upgradeInfos[i];
                upgradeInfo.id = i + 1;
                foreach (var statModifier in upgradeInfo.statModifiers)
                {
                    statModifier.stats = StatNames;
                }

                if (!_isLevelApplied)
                {
                    if (upgradeInfo.applyUpgrades && appliedLevel == -1)
                    {
                        appliedLevel = upgradeInfo.id - 1;
                    }
                }
            }

            if (appliedLevel != -1)
            {
                ApplyUpgrades(appliedLevel);
            }

            if (_isLevelApplied)
            {
                foreach (var statName in StatNames)
                {
                    StatsDictionary[statName].SetDirty();
                }
            }
        }

        public void InitializeRuntime()
        {
            ClearAppliedModifiers();
        }

        public List<StatModifierInfo> GetNextUpgradeInfo()
        {
            int newLevel = _upgradeLevel;
            return upgradeInfos[newLevel].statModifiers;
        }

        public void UpgradeAbility()
        {
            int newLevel = _upgradeLevel;
            ClearAppliedModifiers();
            ApplyUpgrades(newLevel);
        }

        public Stat GetStat(string statName)
        {
            return StatsDictionary[statName];
        }

        private void ApplyUpgrades(int appliedLevel)
        {
            for (int i = 0; i < upgradeInfos.Count; i++)
            {
                var upgradeInfo = upgradeInfos[i];
                if (i <= appliedLevel)
                {
                    upgradeInfo.stateId = 1;
                    foreach (var statModifier in upgradeInfo.statModifiers)
                    {
                        if (statModifier.statName != null && StatsDictionary.ContainsKey(statModifier.statName))
                        {
                            StatsDictionary[statModifier.statName].AddModifier(statModifier.modifier);
                        }
                    }
                }
                else
                {
                    upgradeInfo.stateId = 2;
                }
            }
            _upgradeLevel = appliedLevel + 1;
            _isLevelApplied = true;
        }

        private static List<StatInfo> CreateStatInfosFromNames(string[] names)
        {
            var result = new List<StatInfo>();
            for (int i = 0; i < names.Length; i++)
            {
                result.Add(new StatInfo(names[i]));
            }
            return result;
        }

        protected List<StatInfo> GetAllStatInfos()
        {
            var list = new List<StatInfo>(baseStatInfos);
            list.AddRange(statInfos);
            return list;
        }

        protected string GetStatValuesFormated()
        {
            string formatedStats = "";
            for (int i = 0; i < StatNames.Length; i++)
            {
                string statName = StatNames[i];
                formatedStats += statName + ": " + StatsDictionary[statName].Value;

                if (i != StatNames.Length - 1)
                {
                    formatedStats += "\n";
                }
            }
            return formatedStats;
        }
    }

    [Serializable]
    public class StatInfo
    {
        [HideInInspector] public Stat stat = new();
        [HideInInspector] public bool defaultStat;

        [HideIf("@defaultStat")]
        public string key;
        [ShowIf("@defaultStat"), ReadOnly]
        public string defaultKey;
        public float baseValue;

        public StatInfo(string key = "", float baseValue = 0f, bool defaultStat = false)
        {
            this.key = key;
            this.baseValue = baseValue;

            if (defaultStat)
            {
                this.defaultStat = defaultStat;
                defaultKey = key;
            }
        }
    }

    [Serializable]
    public class UpgradeInfo 
    {
        [HideInInspector] public int id;
        [HideInInspector] public int stateId = 0;

        [Title("$Combined")]
        public List<StatModifierInfo> statModifiers = new();

        [ShowIf("@stateId == 0")]
        public bool applyUpgrades;

        [ShowIf("@stateId == 1")]
        [GUIColor("green")]
        [ReadOnly]
        public string state = "Applied";

        public string Combined { get { return $"Upgrade {id} Info"; } }
    }

    [Serializable]
    public class StatModifierInfo
    {
        [HideInInspector] public string[] stats;

        [ValueDropdown("stats")]
        public string statName;
        [InlineProperty] public StatModifier modifier = new(10f, StatModType.PercentAdd);
    }
}
