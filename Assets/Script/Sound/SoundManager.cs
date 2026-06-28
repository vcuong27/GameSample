using System.Linq;
using UnityEngine;

public class SoundManager : Singleton<SoundManager>
{
    private Sounds sounds;

    public void Initlize()
    {
        sounds = Resources.Load<Sounds>("GameSounds");
    }

    public AudioClip GetAudioClipByType(SoundType type, SoundClip clip)
    {
        return sounds.soundDatas.First(x => x.Type == type && x.Clip == clip).AudioClip;
    }

}
