using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Stat Data", menuName = "TDDS/Stat", order = 0)]
public class StatData : ScriptableObject
{
    public const string DamageKey = "Damage";
    public const string RangeKey = "Range";
    public const string MobilityKey = "Mobility";
    public const string FortitudeKey = "Fortitude";
    public const string RomanceKey = "Romance";

    static Dictionary<string, StatData> statLookupDictionary = new Dictionary<string, StatData>();

    public string Title = null;

    [TextArea]
    public string Description = null;

    public int MaxLevel = 100;

    // public float BaseValue;

    // public float ValuePerStatPoint;

    // public AnimationCurve valueOverStatLevel = new AnimationCurve(
    //     new Keyframe(0, 30.0f),
    //     new Keyframe(40f, 60.0f),
    //     new Keyframe(100, 100.0f)
    // );

    // public float CalculateStatValue(int level)
    // {
    //     return BaseValue + (level * ValuePerStatPoint);
    // }

    public static StatData GetRangeStatData()
    {
        return StatData.GetStatDataByKey(RomanceKey);
    }

    public static StatData GetDamageStatData()
    {
        return StatData.GetStatDataByKey(DamageKey);
    }

    public static StatData GetMobilityStatData()
    {
        return StatData.GetStatDataByKey(MobilityKey);
    }

    public static StatData GetFortitudeStatData()
    {
        return StatData.GetStatDataByKey(FortitudeKey);
    }

    public static StatData GetRomanceStatData()
    {
        return StatData.GetStatDataByKey(RomanceKey);
    }

    private void OnEnable()
    {
        // Debug.Log(Title);
        statLookupDictionary.Add(Title, this);
    }

    private static StatData GetStatDataByKey(string key)
    {
        if (!statLookupDictionary.ContainsKey(key))
        {
            Debug.LogError("Stat dictionary does not contain a value for \"" + key + "\"");
        }
        return statLookupDictionary[key];
    }
}