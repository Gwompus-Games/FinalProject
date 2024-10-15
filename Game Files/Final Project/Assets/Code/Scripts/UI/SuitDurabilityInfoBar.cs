using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SuitDurabilityInfoBar : InfoBarTextElement
{
    [SerializeField] private GameObject _durabilityComponentsObject;
    [SerializeField] private GameObject _durabilitySectionPrefab;
    [SerializeField] private Color _fillColor, _emptyColor;
    List<fillableUISection> _durabilityUISections = new List<fillableUISection>();

    protected override void OnEnable()
    {
        base.OnEnable();
        SuitSystem.UpdateSuitUI += UpdateSuitUI;
        GameManager.Instance.GetManagedComponent<SuitSystem>().UpdateUI();
    }

    protected override void OnDisable()
    {
        base.OnDisable();
        SuitSystem.UpdateSuitUI -= UpdateSuitUI;
    }

    protected override void Start()
    {
        base.Start();
        UpdateText(_uiElementName);
        UpdateSuitUI(GameManager.Instance.GetManagedComponent<SuitSystem>().suitStats.numberOfSections,
                     GameManager.Instance.GetManagedComponent<SuitSystem>().suitStats.numberOfSections,
                     GameManager.Instance.GetManagedComponent<SuitSystem>().suitStats.maxDurabilityForSections,
                     GameManager.Instance.GetManagedComponent<SuitSystem>().suitStats.maxDurabilityForSections);
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
        for (int s = _durabilityUISections.Count - 1; s >= 0; s--)
        {
            Destroy(_durabilityUISections[s].gameObject);
        }
        _durabilityUISections.Clear();
        for (int s = 0; s < numberOfSections; s++)
        {
            _durabilityUISections.Add(Instantiate(_durabilitySectionPrefab, _durabilityComponentsObject.transform).GetComponent<fillableUISection>());
            _durabilityUISections[s].SetFillAmount(1f);
            _durabilityUISections[s].SetColours(_fillColor, _emptyColor);
        }
    }

    public void UpdateSuitUI(int currentSection, int numberOfSections, float currentSectionDurablility, int maxSectionDurability)
    {
        if (!CheckIfSectionTotalMatches(numberOfSections))
        {
            ChangeDurabilityUISections(numberOfSections);
        }
        for (int s = 0; s < _durabilityUISections.Count; s++)
        {
            if (s < currentSection - 1)
            {
                _durabilityUISections[s].SetFillAmount(1f);
            }
            else if (s == currentSection - 1)
            {
                _durabilityUISections[s].SetFillAmount(currentSectionDurablility / (float)maxSectionDurability);
            }
            else
            {
                _durabilityUISections[s].SetFillAmount(0f);
            }
        }
    }
}
