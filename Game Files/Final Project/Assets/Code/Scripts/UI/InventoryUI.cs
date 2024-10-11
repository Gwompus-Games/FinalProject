using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventoryUI : MonoBehaviour
{
    [SerializeField] private GameObject _infoBar;
    private InventoryPopupUI _popupUI;
    private RectTransform _infoBarRectTransform;

    private void Awake()
    {
        if (_infoBar == null)
        {
            throw new System.Exception("Info Bar not assigned to Inventory UI!");
        }
        _popupUI = GetComponentInChildren<InventoryPopupUI>();
        if (_popupUI == null)
        {
            throw new System.Exception("No Popup Found!");
        }
    }

    private void Start()
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
