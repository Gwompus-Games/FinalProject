using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BuySection : MonoBehaviour
{
    [SerializeField] private ToolSO _toolData;
    [SerializeField] private Image _iconImage;
    [SerializeField] private TMP_Text _toolNameText;
    [SerializeField] private TMP_Text _toolCostText;
    [SerializeField] private Image _buyButtonImage;
    [SerializeField] private TMP_Text _buyButtonText;
    private int _buyValue = 0;
    private bool _canAfford = true;
    //private bool _canAfford => PlayerController.Instance.money >= _buyValue;

    private void OnEnable()
    {
        BuyingManager.UpdateBuySections += UpdateSection;
    }

    private void OnDisable()
    {
        BuyingManager.UpdateBuySections -= UpdateSection;
    }

    public void UpdateSection()
    {
        if (_toolData == null)
        {
            throw new System.Exception("No tool data assigned!");
        }
        if (_canAfford)
        {
            _buyButtonImage.color = BuyingManager.Instance.ableToBuyColour;
        }
        else
        {
            _buyButtonImage.color = BuyingManager.Instance.unableToBuyColour;
        }
    }

    public void InitializeSection(ToolSO newToolData)
    {
        _toolData = newToolData;
        _iconImage.sprite = _toolData.shopIcon;
        _toolNameText.text = _toolData.itemName;
        _toolCostText.text = $"${_toolData.buyValue}";
        if (_canAfford)
        {
            _buyButtonImage.color = BuyingManager.Instance.ableToBuyColour;
        }
        else
        {
            _buyButtonImage.color = BuyingManager.Instance.unableToBuyColour;
        }
    }

    public void BuyItem()
    {
        BuyingManager.Instance.BuyItem(_toolData);
    }
}
