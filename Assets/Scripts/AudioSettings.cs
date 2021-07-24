using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioSettings : MonoBehaviour {

     FMOD.Studio.Bus Master;
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
