using System;
using UnityEngine;

public enum SoundType
{
    BGM,
    SFX,
}

public enum SoundClip
{
    BGM_Main,
    BGM_Battle,
    SFX_ButtonClick,
    SFX_Explosion,
}

[Serializable]
public class SoundData
{
    public SoundType Type;
    public SoundClip Clip;
    public AudioClip AudioClip;
}


[CreateAssetMenu(fileName = "Sounds", menuName = "SCR Objects/My Sounds")]

public class Sounds : ScriptableObject
{
    public SoundData[] soundDatas;
}
