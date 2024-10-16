using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventoryUI : ManagedByGameManager
{
    [SerializeField] private GameObject _infoBar;
    public InventoryPopupUI popupUI { get; private set; }
    private RectTransform _infoBarRectTransform;

    public override void Init()
    {
        base.Init();
        if (_infoBar == null)
        {
            throw new System.Exception("Info Bar not assigned to Inventory UI!");
        }
        popupUI = GetComponentInChildren<InventoryPopupUI>();
        if (popupUI == null)
        {
            throw new System.Exception("No Popup Found!");
        }
    }

    public override void CustomStart()
    {
        //SyncBarSize();
    }

    public void SyncBarSize(float width)
    {
        if (_infoBarRectTransform == null)
        {
            _infoBarRectTransform = _infoBar.GetComponent<RectTransform>();
        }
        Vector2 size = _infoBarRectTransform.sizeDelta;
        size.x = width;
        _infoBarRectTransform.sizeDelta = size;
    }
}
