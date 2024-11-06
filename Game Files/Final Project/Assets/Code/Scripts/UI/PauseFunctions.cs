using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class PauseFunctions : ManagedByGameManager
{
    [SerializeField] private GameObject menuPanel;
    [SerializeField] private GameObject optionsPanel;
    [SerializeField] private string mainMenuScene = "";
    private bool paused = false;

    public void Logout()
    {
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(mainMenuScene);
    }

    public void Resume()
    {
        GameManager.Instance.GetManagedComponent<UIManager>().SetUI(UIManager.UIToDisplay.GAME);
        GameManager.Instance.GetManagedComponent<PlayerController>().ChangeState(PlayerController.PlayerState.Idle);
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
