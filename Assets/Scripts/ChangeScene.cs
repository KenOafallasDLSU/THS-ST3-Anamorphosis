using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ChangeScene : MonoBehaviour
{
    public void LoadScene(string sceneName)
    {
        Debug.Log("LOAD SCENE");
        SceneManager.LoadScene(sceneName, LoadSceneMode.Single);
    }

    public void QuitGame()
    {
        Debug.Log("APP QUIT");
        Application.Quit();
    }

    public void UnloadScene(string sceneName)
    {
        Debug.Log("UNLOAD SCENE");
        SceneManager.UnloadSceneAsync(sceneName);
    }
}
