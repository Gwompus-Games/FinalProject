using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class OxygenUI : MonoBehaviour
{
    private TMP_Text _uiText;
    private Image o2Level;
    public Color startColor, endColor;

    private void Awake()
    {
        _uiText = GetComponentInChildren<TMP_Text>();
        o2Level = GetComponentInChildren<Image>();
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
        //if (_uiText == null)
        //{
        //    return;
        //}
        //_uiText.text = textPercent + "%";
        Vector3 o2Scale = o2Level.rectTransform.localScale;
        o2Scale.x = Mathf.Clamp(percentLeft / 100, 0.1f, 1);
        o2Level.rectTransform.localScale = o2Scale;
        o2Level.color = Color.Lerp(startColor, endColor, 1 - (percentLeft / 100));
    }
}
