using UnityEngine;


public enum ClanFlagID
{
    NONE = 0,
    FLAG_1,
    FLAG_2,
    FLAG_3,
    FLAG_4,
}

[System.Serializable]
public class ClanFlagData
{
    public ClanFlagID FlagID;
    public Sprite FlagSprite;
}


[CreateAssetMenu(fileName = "ClanDataReference", menuName = "GameScriptable/ClanDataReference")]

public class ClanDataReference : ScriptableObject
{
    public ClanFlagData[] clanFlagDatas;
}
