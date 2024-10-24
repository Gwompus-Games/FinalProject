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
    private fillableUISection _oxygenUISection;

    protected override void OnEnable()
    {
        if (!_initilized)
        {
            return;
        }
        if (_enabled)
        {
            return;
        }
        base.OnEnable();
        OxygenSystem.OxygenLeftInTank += OxygenUpdateListener;
    }

    protected override void OnDisable()
    {
        if (!_initilized)
        {
            return;
        }
        if (!_enabled)
        {
            return;
        }
        base.OnDisable();
        OxygenSystem.OxygenLeftInTank -= OxygenUpdateListener;
    }

    protected override void Start()
    {
        base.Start();
        _oxygenUISection = Instantiate(_durabilitySectionPrefab, _durabilityComponentsObject.transform).GetComponent<fillableUISection>();
        _oxygenUISection.SetFillAmount(1f);
        _oxygenUISection.SetColours(_fillColor, _emptyColor);
        UpdateText($"{_uiElementName}: %100");
    }

    public override void UpdateText(string textToAdd)
    {
        _uiText.text = textToAdd;
    }
    /* 
     * Old code for multiple sections
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
        int numberOfSections = 1;

        if (!CheckIfSectionTotalMatches(numberOfSections))
        {
            ChangeUISections(numberOfSections);
        }
        for (int s = 0; s < _oxygenUISections.Count; s++)
        {
            _oxygenUISections[s].SetFillAmount(GameManager.OxygenSystemInstance.oxygenTanks[GameManager.OxygenSystemInstance.activeOxygenTank].oxygenFillAmount);
        }
    }
    */
    public void OxygenUpdateListener(string fillPercentString, float fillPercentFloat)
    {
        fillPercentString = fillPercentString.Split('.')[0];
        UpdateText($"{_uiElementName} Current Tank: %{fillPercentString}");
        _oxygenUISection.SetFillAmount(fillPercentFloat / 100f);
    }
}
