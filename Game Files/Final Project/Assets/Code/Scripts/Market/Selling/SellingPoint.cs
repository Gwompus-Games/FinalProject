using UnityEngine;

public class SellingPoint : MonoBehaviour, IInteractable
{
    private InventoryGrid playerInventory;

    private void Awake()
    {
        playerInventory = FindObjectOfType<InventoryTag>().GetComponent<InventoryGrid>();
    }

    private int SellAllTreasuresInInventory()
    {
        InventoryItem[] itemsToSell = playerInventory.GetSellingItems();
        if (itemsToSell.Length == 0)
        {
            return 0;
        }
        int sellValue = 0;
        for (int i = 0; i < itemsToSell.Length; i++)
        {
            sellValue += itemsToSell[i].itemData.baseValue;
        }

        return sellValue;
    }

    public void Interact()
    {
        PlayerController.Instance.GainMoney(SellAllTreasuresInInventory());
    }
}
