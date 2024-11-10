using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class PauseFunctions : MonoBehaviour
{
    [SerializeField] private GameObject menuPanel;
    [SerializeField] private GameObject optionsPanel;
    [SerializeField] private string mainMenuScene = "";
    [SerializeField] private string sceneToUnload = "";
    private bool paused = false;
    private PlayerController _playerController;

    private void Start()
    {
        _playerController = GameManager.Instance.GetManagedComponent<PlayerController>();
    }

    public void Logout()
    {
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(mainMenuScene);
        //seeing if unloading the scene stops the audio from playing
        AsyncOperation asyncUnload = SceneManager.UnloadSceneAsync(sceneToUnload);
    }

    public void Resume()
    {
        _playerController.ChangeUIState(UIManager.UIToDisplay.GAME);
        Time.timeScale = 1;
    }

    public void SwapToReadme()
    {
        optionsPanel.SetActive(true);
    }

    public void SwapToMenu()
    {
        optionsPanel.SetActive(false);
    }
}
