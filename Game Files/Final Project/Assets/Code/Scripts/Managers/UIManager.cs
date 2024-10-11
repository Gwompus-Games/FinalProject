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

    private void Start()
    {
        SetUI(UIToDisplay.GAME);
    }

    public void SetUI(UIToDisplay ui)
    {
        switch (ui)
        {
            case UIToDisplay.GAME:
                Cursor.lockState = CursorLockMode.Locked;
                SetVisable(_inventoryUI.GetComponent<CanvasGroup>(), false);
                SetVisable(_suitUI.GetComponent<CanvasGroup>(), true);
                SetVisable(_shopUI.GetComponent<CanvasGroup>(), false);
                break;
            case UIToDisplay.INVENTORY:
                Cursor.lockState = CursorLockMode.None;
                SetVisable(_inventoryUI.GetComponent<CanvasGroup>(), true);
                SetVisable(_suitUI.GetComponent<CanvasGroup>(), false);
                SetVisable(_shopUI.GetComponent<CanvasGroup>(), false);
                break;
            case UIToDisplay.SHOP:
                Cursor.lockState = CursorLockMode.None;
                SetVisable(_inventoryUI.GetComponent<CanvasGroup>(), true);
                SetVisable(_suitUI.GetComponent<CanvasGroup>(), false);
                SetVisable(_shopUI.GetComponent<CanvasGroup>(), true);
                break;
            default:

                break;
        }
    }

    private void SetVisable(CanvasGroup canvasGroup, bool visable)
    {
        if (canvasGroup == null)
        {
            Debug.LogWarning("Tried to set a null canvas group!");
            return;
        }
        float alpha = 0;
        if (visable)
        {
            alpha = 1;
        }
        canvasGroup.alpha = alpha;
        canvasGroup.interactable = visable;
        canvasGroup.blocksRaycasts = visable;
    }
}
