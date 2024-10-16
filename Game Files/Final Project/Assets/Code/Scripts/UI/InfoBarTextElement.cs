using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public abstract class InfoBarTextElement : ManagedObject
{
    [SerializeField] protected string _uiElementName;
    [SerializeField] protected TMP_Text _uiText;
    protected bool _enabled;

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

    protected virtual void OnEnable()
    {
        if (!_initilized)
        {
            return;
        }
        if (_enabled)
        {
            return;
        }
        _enabled = true;
    }
    
    protected virtual void OnDisable()
    {
        if (!_initilized)
        {
            return;
        }
        if (!_enabled)
        {
            return;
        }
        _enabled = false;
    }

    public override void CustomStart()
    {
        base.CustomStart();
        if (!_enabled)
        {
            OnEnable();
        }
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
