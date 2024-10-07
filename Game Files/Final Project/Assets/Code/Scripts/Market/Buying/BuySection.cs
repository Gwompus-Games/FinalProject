using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UIElements;

public class BuySection : MonoBehaviour
{
    [SerializeField] private ToolSO _toolData;
    [SerializeField] private Image _iconImage;
    [SerializeField] private TMP_Text _toolName;
    [SerializeField] private TMP_Text _toolCost;
    [SerializeField] private Image _buyButton;
    private bool _canAfford => PlayerController.Instance.money >= _toolData.buyValue;

    public void InitializeSection(ToolSO newToolData)
    {
        _toolData = newToolData;
        _iconImage.sprite = _toolData.shopIcon;
        _toolName.text = _toolData.itemName;
        _toolCost.text = $"${_toolData.buyValue}";

    }
}
