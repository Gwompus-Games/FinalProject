using UnityEngine;

public class SellingPoint : InteractableObject
{
    private InventoryGrid playerInventory;

    protected override void Awake()
    {
        base.Awake();
        Canvas[] canvases = GetComponentsInChildren<Canvas>();
        for (int c = 0; c < canvases.Length; c++)
        {
            canvases[c].worldCamera = Camera.main;
        }
    }

    protected override void Start()
    {
        base.Start();
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

    public override void Interact()
    {
        GameManager.Instance.GetManagedComponent<PlayerController>().ChangeUIState(UIManager.UIToDisplay.SHOP);
    }
}
