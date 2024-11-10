using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class MenuFunctions : MonoBehaviour
{
    [SerializeField] private GameObject menuPanel;
    [SerializeField] private GameObject optionsPanel;
    [SerializeField] private string PlayButtonTargetScene = "";
    [SerializeField] private string SceneToUnload = "";

    public void StartGame()
    {
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(PlayButtonTargetScene);
        AsyncOperation asyncUnload = SceneManager.UnloadSceneAsync(SceneToUnload);
    }

    public void SwapToOptions()
    {
        if(menuPanel.activeInHierarchy == true)
        {
            menuPanel.SetActive(false);
            optionsPanel.SetActive(true);
        }
    }

    public void SwapToMenu()
    {
        if (optionsPanel.activeInHierarchy == true)
        {
            menuPanel.SetActive(true);
            optionsPanel.SetActive(false);
        }
    }

    public void ExitGame()
    {
        Application.Quit();
    }
}
