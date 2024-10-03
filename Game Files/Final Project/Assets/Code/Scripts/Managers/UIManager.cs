using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class UIManager : MonoBehaviour
{
    public static UIManager INSTANCE;

    [SerializeField] private GameObject _inventoryUI;
    [SerializeField] private GameObject _suitUI;

    private void Awake()
    {
        if (INSTANCE != null)
        {
            Destroy(gameObject);
            return;
        }
        INSTANCE = this;
    }

    public void SetInventoryUI(bool enabled)
    {
        _inventoryUI.SetActive(enabled);
        _suitUI.SetActive(!enabled);
    }
}
