using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class UIManager : MonoBehaviour
{
    public static UIManager INSTANCE;

    [SerializeField] private GameObject _inventoryUI;
    [SerializeField] private GameObject _suitUI;
    private TMP_Text _suitText;

    private void Awake()
    {
        if (INSTANCE != null)
        {
            Destroy(gameObject);
            return;
        }
        INSTANCE = this;
        _suitText = _suitUI.GetComponentInChildren<TMP_Text>();
    }

    private void OnEnable()
    {
        SuitSystem.UpdateSuitUI += UpdateSuitUI;
    }

    private void OnDisable()
    {
        SuitSystem.UpdateSuitUI -= UpdateSuitUI;
    }

    public void SetInventoryUI(bool enabled)
    {
        _inventoryUI.SetActive(enabled);
        _suitUI.SetActive(!enabled);
    }

    public void UpdateSuitUI()
    {
        string UIText = "Section: " + SuitSystem.INSTANCE.currentSection + "/" + SuitSystem.INSTANCE.numberOfSections + " ";
        UIText += "Durability: " + SuitSystem.INSTANCE.suitDurabilityForCurrentSection + "/" + SuitSystem.INSTANCE.suitDurabilitySectionMax;
        _suitText.text = UIText;
    }
}
