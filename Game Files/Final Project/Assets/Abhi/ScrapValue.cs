using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ScrapValue : MonoBehaviour
{
    private TMP_Text _uiText;

    private void Awake()
    {
        _uiText = GetComponentInChildren<TMP_Text>();
        PlayerController.UpdateMoney += UpdateUI;
    }

    private void OnEnable()
    {
        PlayerController.UpdateMoney += UpdateUI;
    }

    private void OnDisable()
    {
        PlayerController.UpdateMoney -= UpdateUI;
    }

    public void UpdateUI(int money)
    {
        Debug.Log("test");
        print(money);
        if (_uiText == null)
        {
            return;
        }
        _uiText.text = $"${money}";
    }
}
