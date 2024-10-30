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


    public void InitilizeRepairSection(RepairManager.RepairValues values)
    {
        _playerController = GameManager.Instance.GetManagedComponent<PlayerController>();
        UpdateRepairValues(values);
        _sectionTitle.text = values.nameString;
    }

    public void UpdateRepairValues(RepairManager.RepairValues values)
    {
        if (_repairValues.type != null)
        {

        }
        _repairValues = values;
        _sectionText.text = $"${values.currentPrice}";
        _canAfford = values.currentPrice <= _playerController.money;
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
        if (!_canAfford)
        {
            return;
        }
    }
}
