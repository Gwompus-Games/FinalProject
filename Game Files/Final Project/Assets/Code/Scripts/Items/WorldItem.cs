using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MeshRenderer))]
[RequireComponent(typeof(MeshFilter))]
[RequireComponent(typeof(Collider))]
[RequireComponent(typeof(Rigidbody))]
public class WorldItem : MonoBehaviour, IInteractable
{
    [SerializeField] private ItemDataSO _itemData;
    private MeshRenderer _meshRenderer;
    public bool hidden { get; private set; } = false;

    private void Awake()
    {
        _meshRenderer = GetComponent<MeshRenderer>();

        if (_itemData != null)
        {
            AssignData(_itemData);
        }
        transform.tag = "Interactable";
    }

    public void SpawnItem(Vector3 position, ItemDataSO itemData = null)
    {
        if (itemData != null)
        {
            AssignData(itemData);
        }
        transform.position = position;
        HideItem(false);
    }

    public void DespawnItem()
    {
        Destroy(gameObject);
    }

    private void AssignData(ItemDataSO itemData)
    {
        _itemData = itemData;
    }

    private void HideItem(bool hideItem)
    {
        _meshRenderer.enabled = !hideItem;
    }

    public void Interact()
    {
        InventoryController.INSTANCE.AddItemToInventory(_itemData);
        DespawnItem();
    }
}
