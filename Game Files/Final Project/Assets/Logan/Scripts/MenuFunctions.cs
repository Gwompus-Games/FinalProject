using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;
using FMODUnity;

public class MenuFunctions : MonoBehaviour
{
    [SerializeField] public AudioManager audioManager;
    [SerializeField] private GameObject menuPanel;
    [SerializeField] private GameObject optionsPanel;
    [SerializeField] private string PlayButtonTargetScene = "";
    [SerializeField] private EventReference uiClickedSound;
    [SerializeField] private EventReference uiHoveredSound;

    public void StartGame()
    {
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(PlayButtonTargetScene);
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

    public void UIClickSound()
    {
        PlayOneShot(uiClickedSound, transform.position);
    }

    public void UIHoverSound()
    {
        PlayOneShot(uiHoveredSound, transform.position);
        Debug.Log("ButtonHovered");
    }

    public void PlayOneShot(EventReference sound, Vector3 worldPos)
    {
        RuntimeManager.PlayOneShot(sound, worldPos);
    }

    public void ExitGame()
    {
        Application.Quit();
    }
}
