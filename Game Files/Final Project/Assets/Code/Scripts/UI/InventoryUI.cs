using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventoryUI : MonoBehaviour
{
    public static InventoryUI Instance;
    [SerializeField] private GameObject _infoBar;
    private RectTransform _infoBarRectTransform;

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        if (_infoBar == null)
        {
            throw new System.Exception("Info Bar not assigned to Inventory UI!");
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
