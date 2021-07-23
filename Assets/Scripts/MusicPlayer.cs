using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FMODUnity;

public class MusicPlayer : MonoBehaviour
{
    public string levelMusic;
    private static FMOD.Studio.EventInstance Music;

    // Start is called before the first frame update
    void Start()
    {
        StartLevelMusic();
    }

    void Awake()
    {
        DontDestroyOnLoad(this);
    }

    void StartLevelMusic()
    {
    if (levelMusic == "")
        return;
    Music = RuntimeManager.CreateInstance(levelMusic);
    Music.start();
    Music.release();
    }

    public void ChangeMusicParameter(int musicProgressLevel)
    {
        Music.setParameterByName("Music Progress", musicProgressLevel);
    }
}
