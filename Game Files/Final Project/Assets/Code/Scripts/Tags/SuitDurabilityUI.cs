using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class SuitDurabilityUI : MonoBehaviour
{
    private TMP_Text _uiText;

    private void Awake()
    {
        _uiText = GetComponentInChildren<TMP_Text>();
    }

    private void OnEnable()
    {
        SuitSystem.UpdateSuitUI += UpdateUI;
    }

    private void OnDisable()
    {
        SuitSystem.UpdateSuitUI -= UpdateUI;
    }

    private void UpdateUI(int currentSection, int maxSections, float currentDurability, int maxDurabitlity)
    {
        if (_uiText == null)
        {
            return;
        }

        string text = $"{currentSection}/{maxSections} | {currentDurability}/{maxDurabitlity}";

        _uiText.text = text;
    }
}
