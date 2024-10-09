using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SuitDurabilityInfoBar : InfoBarTextElement
{
    [SerializeField] private GameObject _durabilityComponentsObject;
    [SerializeField] private GameObject _durabilitySectionPrefab;
    List<SuitDurabilityUISection> _durabilityUISections = new List<SuitDurabilityUISection>();

    private void OnEnable()
    {
        SuitSystem.UpdateSuitUI += UpdateSuitUI;
    }

    private void OnDisable()
    {
        SuitSystem.UpdateSuitUI -= UpdateSuitUI;
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
        if (_durabilityUISections.Count > 0)
        { 
            for (int i = _durabilityUISections.Count - 1; i >= 0; i--)
            {
                if (_durabilityUISections[i] == null)
                {
                    _durabilityUISections.RemoveAt(i);
                }
            }
        }
        return numberOfSections == _durabilityUISections.Count;
    }

    private void ChangeDurabilityUISections(int numberOfSections)
    {
        if (numberOfSections > _durabilityUISections.Count)
        {

        }
    }

    public void UpdateSuitUI(int currentSection, int numberOfSections, float currentSectionDurablility, int maxSectionDurability)
    {
        if (!CheckIfSectionTotalMatches(numberOfSections))
        {
            ChangeDurabilityUISections(numberOfSections);
        }
    }
}
