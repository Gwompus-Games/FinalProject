using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class RepairSection : MonoBehaviour
{
    [SerializeField] private Image _buttonImage;
    [SerializeField] private TMP_Text _buttonText;
    [SerializeField] private TMP_Text _sectionTitle;
    [SerializeField] private TMP_Text _costText;
    private RepairManager.RepairValues _repairValues;
    private PlayerController _playerController;
    private RepairManager _repairManager;
    private SuitSystem _suitSystem;
    private bool _initialized = false;
    private bool _canBuy
    {
        get
        {
            return _cB;
        }
        set
        {
            _cB = value;
            SetButtonColours();
        }
    }
    private bool _cB;

    private bool _canAfford {
        get
        {
            return _cA;
        }
        set
        {
            _cA = value;
            SetButtonColours();
        }
    }
    private bool _cA;
    private Color _ableToAffordBackgroundColour;
    private Color _ableToAffordTextColour;
    private Color _unableToAffordBackgroundColour;
    private Color _unableToAffordTextColour;
    private Color _cannotPurchaseBackgroundColour;
    private Color _cannotPurchaseTextColour;

    private void OnEnable()
    {
        if (_repairManager != null && _initialized)
        {
            _repairManager.RepairMade += UpdateRepairValues;
            UpdateRepairValues(_repairManager.repairCount);
            switch (_repairValues.type)
            {
                case RepairManager.RepairTypes.REPLACEMENT:
                    SuitSystem.UpdateSuitUI += UpdateRepairSection;
                    break;
            }
        }
    }

    private void OnDisable()
    {
        if (_repairManager != null && _initialized)
        {
            _repairManager.RepairMade -= UpdateRepairValues;
        }
    }

    public void InitilizeRepairSection(RepairManager repairManager, RepairManager.RepairValues values)
    {
        _initialized = true;

        //Set up references
        _repairManager = repairManager;
        _playerController = GameManager.Instance.GetManagedComponent<PlayerController>();
        _suitSystem = GameManager.Instance.GetManagedComponent<SuitSystem>();
        ShopUIManager shopUIManager = GameManager.Instance.GetManagedComponent<ShopUIManager>();

        //Set up colours
        _ableToAffordBackgroundColour = shopUIManager.ableToBuyBackgroundColour;
        _ableToAffordTextColour = shopUIManager.ableToBuyTextColour;
        _unableToAffordBackgroundColour = shopUIManager.unableToBuyBackgroundColour;
        _unableToAffordTextColour = shopUIManager.unableToBuyTextColour;
        _cannotPurchaseBackgroundColour = shopUIManager.cannotBuyBackgroundColour;
        _cannotPurchaseTextColour = shopUIManager.cannotBuyTextColour;

        //Trigger on enable code :^)
        OnEnable();

        //Assign values and colours
        _repairValues.SetupValues(values);
        UpdateRepairValues(0);
        _sectionTitle.text = values.nameString;
        _repairManager.RepairMade += UpdateRepairValues;
    }

    public void UpdateRepairValues(RepairManager.RepairValues values)
    {
        if (!_initialized)
        {
            Debug.LogWarning("Section not initialized!");
            return;
        }
        if (_repairValues.type != values.type)
        {
            Debug.LogWarning("Wrong repair type passed in!");
            return;
        }
        _repairValues = values;
        //Debug.Log($"Updating repair values for {_repairValues.nameString}");
        _costText.text = $"${values.currentPrice}";
        CheckIfCanAfford();
        CheckIfCanBuy();
    }

    public void UpdateRepairValues(int currentAdditiveScale)
    {
        _repairValues.currentPrice = _repairValues.basePrice + (_repairValues.scaleAdditive * currentAdditiveScale);
        UpdateRepairValues(_repairValues);
    }

    private void SetButtonColours()
    {
        if (!_canBuy)
        {
            _buttonImage.color = _cannotPurchaseBackgroundColour;
            _buttonText.color = _cannotPurchaseTextColour;
            return;
        }

        if (_canAfford)
        {
            _buttonImage.color = _ableToAffordBackgroundColour;
            _buttonText.color = _ableToAffordTextColour;
        }
        else
        {
            _buttonImage.color = _unableToAffordBackgroundColour;
            _buttonText.color = _unableToAffordTextColour;
        }
    }

    private bool CheckIfCanBuy()
    {
        switch (_repairValues.type)
        {
            case RepairManager.RepairTypes.MINOR:
                _canBuy = _suitSystem.currentSectionDurability / _suitSystem.maxSectionDurabitity <= _repairValues.minimumPercentToBuy / 100f;
                break;
            case RepairManager.RepairTypes.MAJOR:
                _canBuy = _suitSystem.numberOfSections - _suitSystem.currentSection < _suitSystem.numberOfSections;
                break;
            case RepairManager.RepairTypes.REPLACEMENT:
                _canBuy = _repairManager.repairCount > 0;
                break;
            default:
                _canBuy = false;
                break;
        }
        return _canBuy;
    }

    private bool CheckIfCanAfford()
    {
        _canAfford = _repairValues.currentPrice <= _playerController.money;
        return _canAfford;
    }

    public void ButtonPressed()
    {
        if (!_initialized)
        {
            Debug.LogError("Button pressed before initialization!");
            return;
        }

        if (!CheckIfCanAfford())
        {
            return;
        }

        if (!CheckIfCanBuy())
        {
            return;
        }

        _playerController.SpendMoney(_repairValues.currentPrice);
        _repairManager.RepairBought(_repairValues.type);
        UpdateRepairSection();
    }

    public void UpdateRepairSection()
    {
        CheckIfCanAfford();
        CheckIfCanBuy();
    }

    public void UpdateRepairSection(int currentSection, int numberOfSections, float currentSectionDurability, int maxDurabilityForSections)
    {
        UpdateRepairSection();
    }
}
