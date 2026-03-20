using Mono.Cecil;
using System.Linq;
using UnityEngine;

public class SoundManager : MonoBehaviour
{

    private Sounds sounds;


    private static SoundManager _instance;
    public static SoundManager Instance
    {
        get
        {
            return _instance;
        }
    }

    public void Initlize()
    {
        sounds = Resources.Load<Sounds>("GameSounds");
    }

    public AudioClip GetAudioClipByType(SoundType type, SoundClip clip)
    {
        return sounds.soundDatas.First(x => x.Type == type && x.Clip == clip).AudioClip;
    }



}
