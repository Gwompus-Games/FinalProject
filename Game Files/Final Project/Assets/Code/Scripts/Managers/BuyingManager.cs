using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuyingManager : MonoBehaviour
{
    [field: SerializeField] public ToolListSO toolList { get; private set; }
    public static Action UpdateBuySections;
    [field: SerializeField] public Color ableToBuyColour { get; private set; }
    [field: SerializeField] public Color unableToBuyColour { get; private set; }
    [SerializeField] private GameObject _buySectionPrefab;

    private void Awake()
    {
        
    }

    private void Start()
    {
        CreateBuySections();
    }

    private void OnEnable()
    {
        UpdateBuySections?.Invoke();
    }

    public void CreateBuySections()
    {
        if (toolList == null)
        {
            throw new Exception("No tool list assigned!");
        }
        if (toolList.tools.Count == 0)
        {
            throw new Exception("No tools in the assigned tool list!");
        }

        for (int t = 0; t < toolList.tools.Count; t++)
        {
            BuySection section = Instantiate(_buySectionPrefab, transform).GetComponent<BuySection>();
            section.InitializeSection(toolList.tools[t]);
        }
        UpdateBuySections?.Invoke();
    }

    public void BuyItem(ToolSO toolToBuy)
    {
        if (GameManager.PlayerControllerInstance.money < toolToBuy.buyValue)
        {
            Debug.LogWarning("Player doesn't have enough money to buy this tool.");
            return;
        }
        GameManager.PlayerControllerInstance.SpendMoney(toolToBuy.buyValue);
        GameManager.InventoryControllerInstance.AddItemToInventory(toolToBuy);
        UpdateBuySections?.Invoke();
    }
}
