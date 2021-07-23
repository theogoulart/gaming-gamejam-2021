using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FmodPlayer : MonoBehaviour {

        void PlaySound(string path)
        {
            FMODUnity.RuntimeManager.PlayOneShot(path, GetComponent<Transform>().position);
        }
}
