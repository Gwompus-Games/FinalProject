using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class UIManager : MonoBehaviour
{
    [SerializeField] private GameObject _inventoryUI;
    [SerializeField] private GameObject _suitUI;
    [SerializeField] private GameObject _shopUI;

    public enum UIToDisplay
    {
        GAME,
        INVENTORY,
        SHOP
    }

    private void Awake()
    {

    }

    public void SetUI(UIToDisplay ui)
    {
        switch (ui)
        {
            case UIToDisplay.GAME:
                Cursor.lockState = CursorLockMode.Locked;
                _inventoryUI.SetActive(false);
                _suitUI.SetActive(true);
                _shopUI.SetActive(false);
                break;
            case UIToDisplay.INVENTORY:
                Cursor.lockState = CursorLockMode.None;
                _inventoryUI.SetActive(true);
                _suitUI.SetActive(false);
                _shopUI.SetActive(false);
                break;
            case UIToDisplay.SHOP:
                Cursor.lockState = CursorLockMode.None;
                _inventoryUI.SetActive(true);
                _suitUI.SetActive(false);
                _shopUI.SetActive(true);
                break;
            default:

                break;
        }
    }

    public void SetInventoryUI(bool enabled)
    {
        _inventoryUI.SetActive(enabled);
        _suitUI.SetActive(!enabled);
    }
}
