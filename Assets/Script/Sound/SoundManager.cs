using System.Linq;
using UnityEngine;

public class SoundManager : Singleton<SoundManager>
{
    private Sounds sounds;

    public void Initlize()
    {
        sounds = Resources.Load<Sounds>("GameSounds");
    }

    public AudioClip GetMusicByType(SoundMusicType type)
    {
        return sounds.musicDatas.First(x => x.Type == type).AudioClip;
    }

    public AudioClip GetSFXByType(SoundSFXType type)
    {
        return sounds.sfxDatas.First(x => x.Type == type).AudioClip;
    }

}
