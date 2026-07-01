using System;
using UnityEngine;


public enum SoundMusicType
{
    BGM_Main,
    BGM_Battle,
}

public enum SoundSFXType
{
    SFX_ButtonClick,
    SFX_Explosion,
}


[Serializable]
public class SoundMusicData
{
    public SoundMusicType Type;
    public AudioClip AudioClip;
}

[Serializable]
public class SoundSFXData
{
    public SoundSFXType Type;
    public AudioClip AudioClip;
}


[CreateAssetMenu(fileName = "Sounds", menuName = "GameScriptable/Sounds")]

public class Sounds : ScriptableObject
{
    public SoundMusicData[] musicDatas;
    public SoundSFXData[] sfxDatas;
}
