using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class OxygenInfoBar : InfoBarTextElement
{
    [SerializeField] private GameObject _durabilityComponentsObject;
    [SerializeField] private GameObject _durabilitySectionPrefab;
    [SerializeField] private Color _fillColor, _emptyColor;
    List<fillableUISection> _oxygenUISections = new List<fillableUISection>();

    protected override void OnEnable()
    {
        base.OnEnable();
    }

    protected override void OnDisable()
    {
        base.OnDisable();
    }

    protected override void Start()
    {
        base.Start();
        UpdateText(_uiElementName);
    }

    public override void UpdateText(string textToAdd)
    {
        _uiText.text = textToAdd;
    }

    private bool CheckIfSectionTotalMatches(int numberOfSections)
    {
        if (_oxygenUISections.Count > 0)
        { 
            for (int i = _oxygenUISections.Count - 1; i >= 0; i--)
            {
                if (_oxygenUISections[i] == null)
                {
                    _oxygenUISections.RemoveAt(i);
                }
            }
        }
        return numberOfSections == _oxygenUISections.Count;
    }

    private void ChangeUISections(int numberOfSections)
    {
        for (int s = _oxygenUISections.Count - 1; s >= 0; s--)
        {
            Destroy(_oxygenUISections[s].gameObject);
        }
        _oxygenUISections.Clear();
        for (int s = 0; s < numberOfSections; s++)
        {
            _oxygenUISections.Add(Instantiate(_durabilitySectionPrefab, _durabilityComponentsObject.transform).GetComponent<fillableUISection>());
            _oxygenUISections[s].SetFillAmount(1f);
            _oxygenUISections[s].SetColours(_fillColor, _emptyColor);
        }
    }

    public void UpdateOxygenUI()
    {
        int numberOfSections = GameManager.OxygenSystemInstance.oxygenTanks.Count;

        if (!CheckIfSectionTotalMatches(numberOfSections))
        {
            ChangeUISections(numberOfSections);
        }
        for (int s = 0; s < _oxygenUISections.Count; s++)
        {
            
        }
    }
}
