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
    private Color _canAffordColour;
    private Color _unableToAffordColour;

    private void OnEnable()
    {
        BuyingManager.UpdateBuySections += UpdateSection;
    }

    private void OnDisable()
    {
        BuyingManager.UpdateBuySections -= UpdateSection;
    }

    private void Start()
    {
        _canAffordColour = GameManager.Instance.GetManagedComponent<ShopUIManager>().ableToBuyColour;
        _unableToAffordColour = GameManager.Instance.GetManagedComponent<ShopUIManager>().unableToBuyColour;
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
        if (GameManager.Instance.GetManagedComponent<PlayerController>() != null)
        {
            _canAfford = GameManager.Instance.GetManagedComponent<PlayerController>().money >= _toolData.buyValue;
        }
        if (_canAfford)
        {
            _buyButtonImage.color = _canAffordColour;
        }
        else
        {
            _buyButtonImage.color = _unableToAffordColour;
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
            _buyButtonImage.color = _canAffordColour;
        }
        else
        {
            _buyButtonImage.color = _unableToAffordColour;
        }
        _initialized = true;
    }

    public void BuyItem()
    {
        GameManager.Instance.GetManagedComponent<BuyingManager>().BuyItem(_toolData);
    }
}
