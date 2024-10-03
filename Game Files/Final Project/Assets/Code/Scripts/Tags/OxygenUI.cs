using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class OxygenUI : MonoBehaviour
{
    private TMP_Text _uiText;

    private void Awake()
    {
        _uiText = GetComponentInChildren<TMP_Text>();
    }

    private void OnEnable()
    {
        OxygenSystem.OxygenLeftInTank += UpdateUI;
    }

    private void OnDisable()
    {
        OxygenSystem.OxygenLeftInTank -= UpdateUI;        
    }

    public void UpdateUI(string textPercent, float percentLeft)
    {
        if (_uiText == null)
        {
            return;
        }
        _uiText.text = textPercent + "%";
    }
}
