using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public abstract class InfoBarTextElement : MonoBehaviour
{
    [SerializeField] protected string _uiElementName;
    [SerializeField] protected TMP_Text _uiText;

    protected virtual void Awake()
    {
        if (_uiText == null)
        {
            _uiText = GetComponentInChildren<TMP_Text>();
        }
        if (_uiText == null)
        {
            throw new System.Exception("No text component found!");
        }
    }

    protected virtual void Start()
    {

    }

    public virtual void UpdateText(float number)
    {
        _uiText.text = $"{_uiElementName}: {number}";
    }
    public virtual void UpdateText(int number)
    {
        _uiText.text = $"{_uiElementName}: {number}";
    }

    public virtual void UpdateText(string textToAdd)
    {
        _uiText.text = $"{_uiElementName}: {textToAdd}";
    }
}
