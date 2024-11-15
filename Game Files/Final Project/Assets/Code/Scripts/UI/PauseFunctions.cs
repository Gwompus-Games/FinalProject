using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class PauseFunctions : MonoBehaviour
{
    [SerializeField] private GameObject menuPanel;
    [SerializeField] private GameObject optionsPanel;
    [SerializeField] private GameObject settingsPanel;
    [SerializeField] private string mainMenuScene = "";
    private bool paused = false;
    private PlayerController _playerController;

    private void Start()
    {
        _playerController = GameManager.Instance.GetManagedComponent<PlayerController>();
    }

    public void Logout()
    {
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(mainMenuScene);
    }

    public void Resume()
    {
        _playerController.ChangeUIState(UIManager.UIToDisplay.GAME);
        Time.timeScale = 1;
    }

    public void SwapActivePanels()
    {
        bool menuActive = menuPanel.activeInHierarchy;
        menuPanel.SetActive(!menuActive);
        optionsPanel.SetActive(menuActive);
    }
    
    public void SwapSettingsPanels()
    {
        bool menuActive = menuPanel.activeInHierarchy;
        menuPanel.SetActive(!menuActive);
        settingsPanel.SetActive(menuActive);
    }

}
