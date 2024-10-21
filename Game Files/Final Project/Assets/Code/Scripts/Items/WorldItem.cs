using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MeshRenderer))]
[RequireComponent(typeof(MeshFilter))]
[RequireComponent(typeof(Collider))]
[RequireComponent(typeof(Rigidbody))]
public class WorldItem : InteractableObject
{
    [SerializeField] private ItemDataSO _itemData;
    private MeshRenderer _meshRenderer;
    private Rigidbody _rigidbody;
    public bool hidden { get; private set; } = false;

    protected override void Awake()
    {
        base.Awake();
        _meshRenderer = GetComponent<MeshRenderer>();
        _rigidbody = GetComponent<Rigidbody>();
        if (_itemData != null)
        {
            AssignData(_itemData);
        }
    }

    protected override void Start()
    {
        base.Start();
    }

    public virtual void SpawnItem(Vector3 position, ItemDataSO itemData = null)
    {
        if (itemData != null)
        {
            AssignData(itemData);
        }
        transform.position = position;
        HideItem(false);
    }

    protected void AssignData(ItemDataSO itemData)
    {
        _itemData = itemData;
        _rigidbody.mass = _itemData.density;
        _rigidbody.drag = _itemData.drag;
        _rigidbody.angularDrag = _itemData.angularDrag;
    }

    protected void HideItem(bool hideItem)
    {
        _meshRenderer.enabled = !hideItem;
    }

    public override void Interact()
    {
        GameManager.Instance.GetManagedComponent<InventoryController>().AddItemToInventory(_itemData);
        DespawnItem();
    }
}
