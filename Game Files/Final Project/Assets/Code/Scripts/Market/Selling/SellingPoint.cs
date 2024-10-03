using UnityEngine;

public class SellingPoint : MonoBehaviour, IInteractable
{
    private InventoryGrid playerInventory;

    private void Awake()
    {
        playerInventory = FindObjectOfType<InventoryTag>().GetComponent<InventoryGrid>();
        transform.tag = "Interactable";
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
            Destroy(itemsToSell[i].gameObject);
        }
        return sellValue;
    }

    public void Interact()
    {
        Debug.Log("HELLLOOOO!!!!!");
        PlayerController.Instance.OpenInventory();
        PlayerController.Instance.GainMoney(SellAllTreasuresInInventory());
        PlayerController.Instance.CloseInventory();
    }
}
