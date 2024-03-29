using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(fileName = "MConfig", menuName = "ScriptableObjects/MultipliersConfig", order = 1)]
public class MultipliersConfigSO : ScriptableObject
{
    [Serializable]
    public struct SlotToLevelMult
    {
        [SerializeField] private SlotType _type;
        public List<Pair<Levels, float>> _multipliersByLevel;

        public SlotType Type => _type;

        public float GetMultiplierByLevel(Levels level) => _multipliersByLevel.First(x => x.Key == level).Value;
    }

    [SerializeField] private List<SlotToLevelMult> Gradation;

    public float GetMultiplierOf(SlotType type, Levels level) => Gradation.First(g => g.Type == type).GetMultiplierByLevel(level);
}

[System.Serializable]
public class Pair<K, V>
{

    public Pair(K key, V value)
    {
        Key = key;
        Value = value;
    }

    public K Key;
    public V Value;
}