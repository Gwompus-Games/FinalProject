using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class EndScreenManager : MonoBehaviour
{
    public static EndScreenManager Instance;
    public static Action<EndState> EndStateUpdate;
    public enum EndState
    {
        None,
        Won,
        NoMoneyLeft
    }

    private EndState endedState = EndState.None;
    [SerializeField] private string endSceneName;

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public void SetUpEndState(EndState state)
    {
        endedState = state;
        SceneManager.LoadScene(endSceneName);
    }

    public void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        Debug.Log(scene.name);
        if (scene.name == endSceneName)
        {
            if (endedState == EndState.None)
            {
                throw new Exception($"Ended state not assigned before scene change!");
            }
            Invoke("UpdateEndState", 0.1f);
        }
    }

    private void UpdateEndState()
    {
        EndStateUpdate?.Invoke(endedState);
    }

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;        
    }
}
