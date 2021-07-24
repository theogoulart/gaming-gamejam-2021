using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioSettings : MonoBehaviour {

     FMOD.Studio.EventInstance SFXVolumeTestEvent;

     FMOD.Studio.Bus Music;
     FMOD.Studio.Bus SFX;
     FMOD.Studio.Bus Master;
     float MusicVolume = 0.5f;
     float SFXVolume = 0.5f;
     float MasterVolume = 1f;

     void Awake ()
     {
          Master = FMODUnity.RuntimeManager.GetBus ("bus:/master");
     }

     void Update () 
     {
          Master.setVolume (MasterVolume);
     }

     public void MasterVolumeLevel (float newMasterVolume)
     {
          MasterVolume = newMasterVolume;
     }
}
