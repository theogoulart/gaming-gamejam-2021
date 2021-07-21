using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FMODUnity;

public class MusicManager : MonoBehaviour

{
    private static FMOD.Studio.EventInstance Music;
    public string levelMusic;
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
    public void Progress (int musicProgressLevel)
    {
        Music.setParameterByName("Progress", musicProgressLevel);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    private void OnDestroy()
    {
        Music.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
    }
}
