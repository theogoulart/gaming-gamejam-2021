using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEditor;
using FMODUnity;

public class GameManager : MonoBehaviour
{
    private List<string> scenes = new List<string>();
    public static GameManager instance;
    public int levelIndex;
    public int musicProgressLevel;
    private static FMOD.Studio.EventInstance Music;
    private CameraShake cameraShake;

    // Start is called before the first frame update
    void Awake()
    {
        #if UNITY_EDITOR
        if (scenes.Count == 0) {
            foreach(EditorBuildSettingsScene scene in EditorBuildSettings.scenes)
            {
                if(scene.enabled)
                    scenes.Add(scene.path);
            }
        }
        #else
        scenes.Add("MainMenu");
        scenes.Add("Prototype Tutorial");
        scenes.Add("Prototype 0");
        scenes.Add("Prototype 1");
        scenes.Add("Prototype 2");
        #endif
        instance = this;
        levelIndex = PlayerPrefs.GetInt("LastLevelReached");
        cameraShake = GameObject.FindGameObjectWithTag("CameraPrefab").GetComponent<CameraShake>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void Progress (int musicProgressLevel)
    {
        Music = RuntimeManager.CreateInstance("event:/music/level_music_1");
        Music.setParameterByName("Progress", musicProgressLevel);
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
        LoadLevelScene(++levelIndex);
    }

    public void NewGame()
    {
        LoadLevelScene(0);
    }

    public void GameOver()
    {
        cameraShake.Shake(0.05f, 0.1f);
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