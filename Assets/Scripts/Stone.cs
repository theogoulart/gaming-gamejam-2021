using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Stone : MonoBehaviour
{
    private List<GameObject> platforms = new List<GameObject>();
    private List<GameObject> platformsToDisable = new List<GameObject>();
    public string color;

    private void Awake()
    {
        GameObject[] allPlatforms = GameObject.FindGameObjectsWithTag("Platform");

        foreach (var platform in allPlatforms)
        {
            if (platform.GetComponent<Platform>().color == color) {
                platforms.Add(platform);
            } else {
                platformsToDisable.Add(platform);
            }
        }
    }

    public void Pick()
    {
        FMODUnity.RuntimeManager.PlayOneShot("event:/sfx/char/char_transform", transform.position);
        Debug.Log(platforms.Count);
        foreach (var p in platforms)
        {
            p.SetActive(true);
        }

        foreach (var p in platformsToDisable)
        {
            p.SetActive(false);
        }
    }
}
