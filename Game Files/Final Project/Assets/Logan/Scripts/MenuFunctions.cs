using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;
using FMODUnity;
using UnityEngine.Rendering;
using UnityEngine.Rendering.HighDefinition;

public class MenuFunctions : MonoBehaviour
{
    [SerializeField] private GameObject menuPanel;
    [SerializeField] private GameObject optionsPanel;
    [SerializeField] private GameObject settingsPanel;
    [SerializeField] private string PlayButtonTargetScene = "";
    [SerializeField] private Volume CRT;


    public void StartGame()
    {
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(PlayButtonTargetScene);
    }

    public void SwapActivePanelsMM()
    {
        bool menuActive = menuPanel.activeInHierarchy;
        menuPanel.SetActive(!menuActive);
        optionsPanel.SetActive(menuActive);
    }
    
    public void SwapActivePanelsSettings()
    {
        bool menuActive = menuPanel.activeInHierarchy;
        menuPanel.SetActive(!menuActive);
        settingsPanel.SetActive(menuActive);
        if(menuActive)
        {
            CRT.weight = 0;
        }
        else
        {
            CRT.weight = 1;
        }
    }


    public void UIClickSound()
    {
        AudioManager.Instance.OnClick();
    }

    public void UIHoverSound()
    {
        AudioManager.Instance.OnHover();
    }

    public void ExitGame()
    {
        Application.Quit();
    }
}
