using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class EndScreenButton : MonoBehaviour
{
    public void ButtonPressed()
    {
        SceneManager.LoadScene(0);
    }
}
