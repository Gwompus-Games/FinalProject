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
    private bool _initialized = false;
    private bool _canAfford = false;

    private void OnEnable()
    {
        BuyingManager.UpdateBuySections += UpdateSection;
        UpdateSection();
    }

    private void OnDisable()
    {
        BuyingManager.UpdateBuySections -= UpdateSection;
    }

    public void UpdateSection()
    {
        if (!_initialized)
        {
            return;
        }
        if (_toolData == null)
        {
            throw new System.Exception("No tool data assigned!");
        }
        if (GameManager.PlayerControllerInstance != null)
        {
            _canAfford = GameManager.PlayerControllerInstance.money >= _toolData.buyValue;
        }
        if (_canAfford)
        {
            _buyButtonImage.color = GameManager.BuyingManagerInstance.ableToBuyColour;
        }
        else
        {
            _buyButtonImage.color = GameManager.BuyingManagerInstance.unableToBuyColour;
        }
    }

    public void InitializeSection(ToolSO newToolData)
    {
        if (_initialized)
        {
            return;
        }
        _toolData = newToolData;
        _iconImage.sprite = _toolData.shopIcon;
        _toolNameText.text = _toolData.itemName;
        _toolCostText.text = $"${_toolData.buyValue}";
        if (_canAfford)
        {
            _buyButtonImage.color = GameManager.BuyingManagerInstance.ableToBuyColour;
        }
        else
        {
            _buyButtonImage.color = GameManager.BuyingManagerInstance.unableToBuyColour;
        }
        _initialized = true;
    }

    public void BuyItem()
    {
        GameManager.BuyingManagerInstance.BuyItem(_toolData);
    }
}
