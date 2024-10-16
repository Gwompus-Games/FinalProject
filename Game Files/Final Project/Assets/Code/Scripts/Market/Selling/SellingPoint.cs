using UnityEngine;

public class SellingPoint : MonoBehaviour, IInteractable
{
    private InventoryGrid playerInventory;

    private void Awake()
    {
        transform.tag = "Interactable";
        GetComponentInChildren<Canvas>().worldCamera = Camera.main;
    }

    private void Start()
    {
        GameManager.Instance.GetManagedComponent<PlayerController>().OpenInventory();
        playerInventory = FindObjectOfType<InventoryTag>().GetComponent<InventoryGrid>();
        GameManager.Instance.GetManagedComponent<PlayerController>().CloseInventory();
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
        GameManager.Instance.GetManagedComponent<PlayerController>().ChangeUIState(UIManager.UIToDisplay.SHOP);
    }
}
