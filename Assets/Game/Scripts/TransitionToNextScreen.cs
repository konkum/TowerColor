using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
public class TransitionToNextScreen : MonoBehaviour
{
    public Slider slider;
    private AsyncOperation scene;

    public static System.Action OnLoadToNextLevel;

    private void Start()
    {
        OnLoadToNextLevel += LoadNextLevel;
    }

    private void LoadNextLevel()
    {
        scene = SceneManager.LoadSceneAsync("Game");
    }

    private void Update()
    {
        if (scene != null)
        {
            slider.value = scene.progress;
        }
    }

    private void OnDestroy()
    {
        OnLoadToNextLevel -= LoadNextLevel;
    }
}
