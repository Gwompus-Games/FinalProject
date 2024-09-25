using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class UIManager : MonoBehaviour
{
    [SerializeField] private TMP_Text _suitText;

    private void OnEnable()
    {
        SuitSystem.UpdateSuitUI += UpdateSuitUI;
    }

    private void OnDisable()
    {
        SuitSystem.UpdateSuitUI -= UpdateSuitUI;
    }

    public void UpdateSuitUI()
    {
        string UIText = "Section: " + SuitSystem.INSTANCE.currentSection + "/" + SuitSystem.INSTANCE.numberOfSections + " ";
        UIText += "Durability: " + SuitSystem.INSTANCE.suitDurabilityForCurrentSection + "/" + SuitSystem.INSTANCE.suitDurabilitySectionMax;
        _suitText.text = UIText;
    }
}
