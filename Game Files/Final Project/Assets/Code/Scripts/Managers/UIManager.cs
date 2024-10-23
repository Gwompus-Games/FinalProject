using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System.Linq;

public class UIManager : ManagedByGameManager
{
    [SerializeField] private GameObject _inventoryUI;
    [SerializeField] private GameObject _suitUI;
    [SerializeField] private GameObject _shopUI;
    private List<ManagedObject> _managedObjects = new List<ManagedObject>();

    public enum UIToDisplay
    {
        GAME,
        INVENTORY,
        SHOP
    }

    public override void Init()
    {
        base.Init();
        List<InfoBarTextElement> infoElements = new List<InfoBarTextElement>(GetComponentsInChildren<InfoBarTextElement>());
        for (int i = 0; i < infoElements.Count; i++)
        {
            Debug.Log(infoElements.GetType());
        }
        List<InventoryGrid> inventoryGrids = new List<InventoryGrid>(GetComponentsInChildren<InventoryGrid>());
        _managedObjects = new List<ManagedObject>();
        _managedObjects.Concat(infoElements);
        _managedObjects.Concat(inventoryGrids);
    }

    public override void CustomStart()
    {
        base.CustomStart();
        for (int ibe = 0; ibe < _managedObjects.Count; ibe++)
        {
            _managedObjects[ibe].Init();
        }
        for (int ibe = 0; ibe < _managedObjects.Count; ibe++)
        {
            _managedObjects[ibe].CustomStart();
        }
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
