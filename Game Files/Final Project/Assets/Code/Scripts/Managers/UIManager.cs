using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System.Linq;
using System;
using System.Reflection;
using UnityEngine.UI;
using UnityEngine.Rendering;

public class UIManager : ManagedByGameManager
{
    [SerializeField] private GameObject _inventoryUI;
    [SerializeField] private GameObject _suitUI;
    [SerializeField] private GameObject _shopUI;
    [SerializeField] private GameObject _pauseUI;
    private List<ManagedObject> _managedObjects = new List<ManagedObject>();
    [SerializeField] private bool _debugMode = false;
    [SerializeField] private Volume crtShader;
    [SerializeField] private float crtShaderWeight = 0.75f;

    public enum UIToDisplay
    {
        GAME,
        INVENTORY,
        SHOP,
        PAUSE
    }

    private void Awake()
    {
        //crtShader = GameObject.Find("CRT_PostProcess_Prefab").GetComponent<Volume>();
        _pauseUI = GameObject.Find("PauseUI");
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
        crtShader.weight = crtShaderWeight;

        switch (ui)
        {
            case UIToDisplay.GAME:
                crtShader.weight = 0f;
                Cursor.lockState = CursorLockMode.Locked;
                SetVisible(_inventoryUI.GetComponent<CanvasGroup>(), false);
                SetVisible(_suitUI.GetComponent<CanvasGroup>(), true);
                SetVisible(_shopUI.GetComponent<CanvasGroup>(), false);
                SetVisible(_pauseUI.GetComponent<CanvasGroup>(), false);
                break;
            case UIToDisplay.PAUSE:
                Cursor.lockState = CursorLockMode.None;
                SetVisible(_inventoryUI.GetComponent<CanvasGroup>(), false);
                SetVisible(_suitUI.GetComponent<CanvasGroup>(), false);
                SetVisible(_shopUI.GetComponent<CanvasGroup>(), false);
                SetVisible(_pauseUI.GetComponent<CanvasGroup>(), true);
                break;
            case UIToDisplay.INVENTORY:
                Cursor.lockState = CursorLockMode.None;
                SetVisible(_inventoryUI.GetComponent<CanvasGroup>(), true);
                SetVisible(_suitUI.GetComponent<CanvasGroup>(), false);
                SetVisible(_shopUI.GetComponent<CanvasGroup>(), false);
                SetVisible(_pauseUI.GetComponent<CanvasGroup>(), false);
                break;
            case UIToDisplay.SHOP:
                Cursor.lockState = CursorLockMode.None;
                SetVisible(_inventoryUI.GetComponent<CanvasGroup>(), true);
                SetVisible(_suitUI.GetComponent<CanvasGroup>(), false);
                SetVisible(_shopUI.GetComponent<CanvasGroup>(), true);
                SetVisible(_pauseUI.GetComponent<CanvasGroup>(), false);
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

    private void SetVisible(CanvasGroup canvasGroup, bool visible)
    {
        if (canvasGroup == null)
        {
            Debug.LogWarning("Tried to set a null canvas group!");
            return;
        }
        float alpha = 0;
        if (visible)
        {
            alpha = 1;
        }
        canvasGroup.alpha = alpha;
        canvasGroup.interactable = visible;
        canvasGroup.blocksRaycasts = visible;
    }
}
