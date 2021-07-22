using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraShake : MonoBehaviour
{
    public Camera mainCam;
    float shakeAmount = 0;

    // Start is called before the first frame update
    void Awake()
    {
        if (mainCam == null) {
            mainCam = Camera.main;
        }
    }

    public void Shake(float amt, float length)
    {
        shakeAmount = amt;
        InvokeRepeating("StartShaking", 0, 0.01f);
        Invoke("StopShaking", length);
    }

    void StartShaking()
    {
        if (shakeAmount > 0) {
            Vector3 camPos = mainCam.transform.position;

            float offsetX = Random.value * shakeAmount * 2 - shakeAmount;
            float offsetY = Random.value * shakeAmount * 2 - shakeAmount;
            camPos.x += offsetX;
            camPos.y += offsetY;

            mainCam.transform.position = camPos;
        }
    }

    void StopShaking()
    {
        CancelInvoke("StartShaking");
        mainCam.transform.localPosition = Vector3.zero;
    }
}
