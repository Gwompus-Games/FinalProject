using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BuyingManager : ManagedByGameManager
{
    [field: SerializeField] public ToolListSO toolList { get; private set; }
    public static Action UpdateBuySections;
    
    [Header("\"Freedom\" Values")]
    [SerializeField] private Image _freedomButtonImage;
    [SerializeField] private TMP_Text _freedomButtonText;
    [SerializeField] private int _freedomCost = 1000000;
    [SerializeField] private TMP_Text _freedomCostText;
    private bool _ableToAffordFreedom
    {
        get
        {
            return _aAF;
        }
        set
        {
            _aAF = value;
            UpdateButtonColour();
        }
    }
    private bool _aAF;

    [Header("Section Variables")]
    [SerializeField] private GameObject _buySectionPrefab;

    private PlayerController _playerController;
    private ShopUIManager _shopUIManager;
    private InventoryController _inventoryController;

    private Color _ableToAffordBackgroundColour;
    private Color _ableToAffordTextColour;
    private Color _unableToAffordBackgroundColour;
    private Color _unableToAffordTextColour;

    public override void Init()
    {
        base.Init();
        _playerController = GameManager.Instance.GetManagedComponent<PlayerController>();
        _shopUIManager = GameManager.Instance.GetManagedComponent<ShopUIManager>();
        _inventoryController = GameManager.Instance.GetManagedComponent<InventoryController>();

        OnEnable();
    }

    public override void CustomStart()
    {
        _freedomCostText.text = $"${_freedomCost.ToString("N0")}";

        _ableToAffordBackgroundColour = _shopUIManager.ableToBuyBackgroundColour;
        _ableToAffordTextColour = _shopUIManager.ableToBuyTextColour;
        _unableToAffordBackgroundColour = _shopUIManager.unableToBuyBackgroundColour;
        _unableToAffordTextColour = _shopUIManager.unableToBuyTextColour;

        CreateBuySections();
    }

    private void OnEnable()
    {
        UpdateBuySections?.Invoke();

        PlayerController.UpdateMoney += UpdateMoney;
        if (_playerController != null)
        {
            UpdateMoney(_playerController.money);
        }
    }

    private void OnDisable()
    {
        PlayerController.UpdateMoney -= UpdateMoney;
    }

    public void CreateBuySections()
    {
        if (toolList == null)
        {
            throw new Exception("No tool list assigned!");
        }
        if (toolList.tools.Count == 0)
        {
            throw new Exception("No tools in the assigned tool list!");
        }

        for (int t = 0; t < toolList.tools.Count; t++)
        {
            BuySection section = Instantiate(_buySectionPrefab, transform).GetComponent<BuySection>();
            section.InitializeSection(toolList.tools[t]);
        }
        UpdateBuySections?.Invoke();
    }

    public void BuyItem(ToolSO toolToBuy)
    {
        if (_playerController.money < toolToBuy.buyValue)
        {
            Debug.LogWarning("Player doesn't have enough money to buy this tool.");
            return;
        }
        _playerController.SpendMoney(toolToBuy.buyValue);
        OxygenTankSO oxygenTank = toolToBuy as OxygenTankSO;
        if (oxygenTank != null)
        {
            _inventoryController.AddItemToInventory(oxygenTank, float.MaxValue);
        }
        else
        {
            _inventoryController.AddItemToInventory(toolToBuy);
        }
        UpdateBuySections?.Invoke();
    }

    public void UpdateMoney(int moneyValue)
    {
        _ableToAffordFreedom = moneyValue >= _freedomCost;
    }

    private void UpdateButtonColour()
    {
        if (_ableToAffordFreedom)
        {
            _freedomButtonImage.color = _ableToAffordBackgroundColour;
            _freedomButtonText.color = _ableToAffordTextColour;
        }
        else
        {
            _freedomButtonImage.color = _unableToAffordBackgroundColour;
            _freedomButtonText.color = _unableToAffordTextColour;
        }
    }

    public void BuyFreedom()
    {
        if (_playerController.money < _freedomCost)
        {
            return;
        }

        _playerController.KillPlayer(DeathObject.DeathType.Won);
    }
}
