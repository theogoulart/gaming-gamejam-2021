using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEditor;
using UnityEngine.UI;
using FMODUnity;
using TMPro;

public class GameManager : MonoBehaviour
{
    private List<string> scenes = new List<string>();
    public static GameManager instance;
    public int levelIndex;
    public int musicProgressLevel;
    private CameraShake cameraShake;
    private int deathCount;

    // Start is called before the first frame update
    void Awake()
    {
        if (instance == null) {
            instance = this;
        }

        // #if UNITY_EDITOR
        // if (scenes.Count == 0) {
        //     foreach(EditorBuildSettingsScene scene in EditorBuildSettings.scenes)
        //     {
        //         if(scene.enabled)
        //             scenes.Add(scene.path);
        //     }
        // }
        // #else
        scenes.Add("MainMenu");
        scenes.Add("TESTELEVEL1");
        scenes.Add("TESTELEVEL2");
        scenes.Add("TESTELEVEL3");
        scenes.Add("TESTELEVEL4");
        scenes.Add("TESTELEVEL5");
        scenes.Add("TESTELEVEL6");
        scenes.Add("TESTELEVEL7");
        scenes.Add("TESTELEVEL8");
        scenes.Add("TESTELEVEL9");
        scenes.Add("TESTELEVEL10");
        // #endif
        levelIndex = PlayerPrefs.GetInt("LastLevelReached");
        deathCount = PlayerPrefs.GetInt("DeathCount");

        GameObject camShakeObj = GameObject.FindGameObjectWithTag("CameraPrefab");
        if (camShakeObj != null) {
            try {
                cameraShake = camShakeObj.GetComponent<CameraShake>();
            } catch (System.Exception e) {
                Debug.Log("no camera shake" + e.Message);
            }
        }
    }

    void Start()
    {
        // FindObjectOfType<MusicPlayer>().ChangeMusicParameter(musicProgressLevel); 
        GameObject[] allPlatforms = GameObject.FindGameObjectsWithTag("Platform");

        foreach (var platform in allPlatforms)
        {
            if (platform.GetComponent<Platform>().color != "Gray") {
                platform.SetActive(false);
            }
        }
    }

    public void Continue()
    {
        LoadLevelScene(levelIndex);
    }

    public void LoadLevelScene(int index)
    {
        index = index <= 0 || index > SceneManager.sceneCountInBuildSettings ? 1 : index;
        Debug.Log("Load " + index);
        PlayerPrefs.SetInt("LastLevelReached", index);
        SceneManager.LoadScene(scenes[index]);
    }

    public void NextLevel()
    {
        try {
            LoadLevelScene(++levelIndex);
        } catch (System.Exception excpt) {
            RestartLevel();
        }
    }

    public void NewGame()
    {
        PlayerPrefs.SetInt("DeathCount", 0);
        LoadLevelScene(0);
    }

    public void GameOver()
    {
        cameraShake.Shake(0.05f, 0.1f);
        PlayerPrefs.SetInt("DeathCount", ++deathCount);
        StartCoroutine(OnGameOver());
    }

    IEnumerator OnGameOver()
    {
        yield return new WaitForSeconds(.5f);
        RestartLevel();
    }

    public void RestartLevel()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}