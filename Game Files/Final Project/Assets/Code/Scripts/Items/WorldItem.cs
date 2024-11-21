using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[RequireComponent(typeof(Rigidbody))]
public class WorldItem : InteractableObject
{
    [SerializeField] protected ItemDataSO _itemData;
    [SerializeField] private GameObject _pickupEffect;
    private MeshRenderer _meshRenderer;
    private Rigidbody _rigidbody;
    public bool hidden { get; private set; } = false;
    private float _pushForce;

    protected override void Awake()
    {
        base.Awake();
        _meshRenderer = GetComponentInChildren<MeshRenderer>();
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
        _rigidbody.useGravity = !_itemData.floatingItem;
        _pushForce = 80f / (_itemData.density * _itemData.drag);
    }

    public void DespawnItem()
    {
        Destroy(gameObject);
    }

    protected void HideItem(bool hideItem)
    {
        _meshRenderer.enabled = !hideItem;
    }

    public override void Interact()
    {
        AudioManager.Instance.OnPickup();
        GameManager.Instance.GetManagedComponent<InventoryController>().AddItemToInventory(_itemData);
        if (_pickupEffect != null)
        {
            Instantiate(_pickupEffect, transform.parent).transform.localPosition = transform.localPosition;
        }
        DespawnItem();
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.tag == "Player")
        {
            Vector3 direction = (transform.position - other.transform.position).normalized;
            Vector3 force = direction * _pushForce;
            _rigidbody.AddForce(force);
        }
    }
}
