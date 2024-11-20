using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(SellingUIInteract))]
public class SellingZone : MonoBehaviour
{
    [SerializeField] private SellValueUIScript _uiComponent;

    private void Start()
    {
        _uiComponent.UpdateUI(0);
    }

    public SellValueUIScript GetSellComponentUI()
    {
        return _uiComponent;
    }

    public void SellItem(InventoryItem itemToSell)
    {
        if (itemToSell == null)
        {
            return;
        }
        if (itemToSell.itemData == null)
        {
            return;
        }

        //Add sound effect for selling here

        GameManager.Instance.GetManagedComponent<PlayerController>().GainMoney(itemToSell.sellValue);
        _uiComponent.UpdateUI(0);
        itemToSell.ItemRemovedFromInventory();
        Destroy(itemToSell.gameObject);
    }
}
