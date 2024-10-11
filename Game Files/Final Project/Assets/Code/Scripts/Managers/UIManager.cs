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
                _inventoryUI.GetComponent<CanvasGroup>().alpha = 0;
                _suitUI.GetComponent<CanvasGroup>().alpha = 1;
                _shopUI.GetComponent<CanvasGroup>().alpha = 0;
                break;
            case UIToDisplay.INVENTORY:
                Cursor.lockState = CursorLockMode.None;
                _inventoryUI.GetComponent<CanvasGroup>().alpha = 1;
                _suitUI.GetComponent<CanvasGroup>().alpha = 0;
                _shopUI.GetComponent<CanvasGroup>().alpha = 0;
                break;
            case UIToDisplay.SHOP:
                Cursor.lockState = CursorLockMode.None;
                _inventoryUI.GetComponent<CanvasGroup>().alpha = 1;
                _suitUI.GetComponent<CanvasGroup>().alpha = 0;
                _shopUI.GetComponent<CanvasGroup>().alpha = 1;
                break;
            default:

                break;
        }
    }
}
