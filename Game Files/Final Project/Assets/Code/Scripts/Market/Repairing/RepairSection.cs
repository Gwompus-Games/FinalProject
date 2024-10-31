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
    [SerializeField] private TMP_Text _sectionText;
    private RepairManager.RepairValues _repairValues;
    private PlayerController _playerController;
    private RepairManager _repairManager;

    private bool _canAfford {
        get
        {
            return _cA;
        }
        set
        {
            _cA = value;
            SetButtonColour();
        }
    }
    private bool _cA;
    private Color _canAffordColour;
    private Color _unableToAffordColor;

    private void OnEnable()
    {
        if (_repairManager != null && _repairValues.initialized)
        {
            _repairManager.RepairMade += UpdateRepairValues;
        }
    }

    private void OnDisable()
    {
        if (_repairManager != null && _repairValues.initialized)
        {
            _repairManager.RepairMade -= UpdateRepairValues;
        }
    }

    public void InitilizeRepairSection(RepairManager repairManager, RepairManager.RepairValues values)
    {
        _repairManager = repairManager;
        _playerController = GameManager.Instance.GetManagedComponent<PlayerController>();
        UpdateRepairValues(values);
        _sectionTitle.text = values.nameString;
        _repairManager.RepairMade += UpdateRepairValues;
    }

    public void UpdateRepairValues(RepairManager.RepairValues values)
    {
        if (!_repairValues.initialized)
        {
            return;
        }
        if (_repairValues.type != values.type)
        {
            return;
        }
        _repairValues = values;
        _sectionText.text = $"${values.currentPrice}";
        _canAfford = values.currentPrice <= _playerController.money;
    }

    public void UpdateRepairValues(int currentAdditiveScale)
    {
        _repairValues.currentPrice = _repairValues.basePrice + (_repairValues.scaleAdditive * currentAdditiveScale);
        UpdateRepairValues(_repairValues);
    }

    private void SetButtonColour()
    {
        if (_canAfford)
        {
            _buttonImage.color = _canAffordColour;
        }
        else
        {
            _buttonImage.color = _unableToAffordColor;
        }
    }

    public void ButtonPressed()
    {
        if (!_repairValues.initialized)
        {
            return;
        }

        if (!_canAfford)
        {
            return;
        }

        if (_playerController.money < _repairValues.currentPrice)
        {
            _canAfford = false;
            return;
        }

        _playerController.SpendMoney(_repairValues.currentPrice);
        _repairManager.RepairBought(_repairValues.type);
    }
}
