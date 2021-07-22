using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FMODUnity;

public class MusicManager : MonoBehaviour

{
    private static FMOD.Studio.EventInstance Music;
    public string levelMusic;
    public int musicProgressLevel;
    public 
    // Start is called before the first frame update
    void Start()
    {
        StartLevelMusic();
    }
    void StartLevelMusic()
    {
        if (levelMusic == "")
            return;
        Music = RuntimeManager.CreateInstance(levelMusic);
        Music.start();
        Music.release();
    }
    // public void Progress (int musicProgressLevel)
    // {
    //     Music.setParameterByName("Music Progress", musicProgressLevel);
    // }

    // Update is called once per frame
    void Update()
    {
        Music.setParameterByName("Music Progress", musicProgressLevel);
        Debug.Log("parameter changed");
    }
    // private void OnDestroy()
    // {
    //     Music.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
    // }
}
