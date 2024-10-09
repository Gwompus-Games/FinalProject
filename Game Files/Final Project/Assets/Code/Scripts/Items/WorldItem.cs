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
    private Rigidbody _rigidbody;
    public bool hidden { get; private set; } = false;

    private void Awake()
    {
        _meshRenderer = GetComponent<MeshRenderer>();
        _rigidbody = GetComponent<Rigidbody>();
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
        _rigidbody.mass = _itemData.density;
        _rigidbody.drag = _itemData.drag;
        _rigidbody.angularDrag = _itemData.angularDrag;
    }

    private void HideItem(bool hideItem)
    {
        _meshRenderer.enabled = !hideItem;
    }

    public void Interact()
    {
        InventoryController.Instance.AddItemToInventory(_itemData);
        DespawnItem();
    }
}
