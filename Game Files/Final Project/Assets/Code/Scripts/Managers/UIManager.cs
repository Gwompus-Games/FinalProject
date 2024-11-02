using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System.Linq;
using System;
using System.Reflection;

public class UIManager : ManagedByGameManager
{
    [SerializeField] private GameObject _inventoryUI;
    [SerializeField] private GameObject _suitUI;
    [SerializeField] private GameObject _shopUI;
    private List<ManagedObject> _managedObjects = new List<ManagedObject>();
    [SerializeField] private bool _debugMode = false;

    public enum UIToDisplay
    {
        GAME,
        INVENTORY,
        SHOP
    }

    public override void Init()
    {
        base.Init();
        if (_debugMode)
        {
            Debug.Log("UI Manager Initilized");
        }
        List<Type> infoElementChildren = Assembly.GetAssembly(typeof(InfoBarTextElement)).GetTypes().Where(t => t.IsSubclassOf(typeof(InfoBarTextElement))).ToList();
        List<InfoBarTextElement> infoElements = new List<InfoBarTextElement>();
        for (int t = 0; t < infoElementChildren.Count; t++)
        {
            infoElements.Add(GetComponentInChildren(infoElementChildren[t]) as InfoBarTextElement);
        }
        List<InventoryGrid> inventoryGrids = new List<InventoryGrid>(GetComponentsInChildren<InventoryGrid>());
        _managedObjects = new List<ManagedObject>();
        _managedObjects = _managedObjects.Concat(infoElements).ToList();
        _managedObjects = _managedObjects.Concat(inventoryGrids).ToList();
        if (_debugMode)
        {
            Debug.Log($"Found a total of {_managedObjects.Count}  objects.");
        }
        for (int ibe = 0; ibe < _managedObjects.Count; ibe++)
        {
            if (_debugMode)
            {
                Debug.Log($"{_managedObjects[ibe].gameObject.name} is being initilized by {gameObject.name}");
            }
            _managedObjects[ibe].Init();
            if (_debugMode)
            {
                Debug.Log($"{_managedObjects[ibe].gameObject.name} initilized!");
            }
        }
    }

    public override void CustomStart()
    {
        base.CustomStart();
        for (int ibe = 0; ibe < _managedObjects.Count; ibe++)
        {
            if (_debugMode)
            {
                Debug.Log($"{_managedObjects[ibe].gameObject.name} is being Started by {gameObject.name}!");
            }
            _managedObjects[ibe].CustomStart();
            if (_debugMode)
            {
                Debug.Log($"{_managedObjects[ibe].gameObject.name} Started!");
            }
        }
        SetUI(UIToDisplay.GAME);
    }

    public void SetUI(UIToDisplay ui)
    {
        RepairManager repairManager = null;

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
                repairManager = FindObjectOfType<RepairManager>();
                break;
            default:

                break;
        }

        if (repairManager != null)
        {
            repairManager.UpdateAllSections();
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
