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

    public void SwapActivePanelsMM()
    {
        bool menuActive = menuPanel.activeInHierarchy;
        menuPanel.SetActive(!menuActive);
        optionsPanel.SetActive(menuActive);
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
