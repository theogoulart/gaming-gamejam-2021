using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Coin : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void PickCoin()
    {
        GameObject[] walls = GameObject.FindGameObjectsWithTag("RemovableWall");
        foreach (var wall in walls)
        {
            wall.SetActive(false);
        }
        Destroy(gameObject);
    }
}
