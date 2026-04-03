using UnityEngine;

[System.Serializable]
public class LootRarityChances
{
    [Range(0, 100)] public int common;
    [Range(0, 100)] public int uncommon;
    [Range(0, 100)] public int rare;
    [Range(0, 100)] public int epic;
    [Range(0, 100)] public int legendary;
}