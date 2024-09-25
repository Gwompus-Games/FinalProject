using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MeshRenderer))]
[RequireComponent(typeof(MeshFilter))]
public class WorldItem : MonoBehaviour
{
    [SerializeField] private ItemDataSO _itemData;
    private MeshRenderer _meshRenderer;
    public bool hidden { get; private set; } = false;

    private void Awake()
    {
        if (_itemData != null)
        {
            AssignData(_itemData);
        }
        _meshRenderer = GetComponent<MeshRenderer>();
    }

    public void SpawnItem(Vector3 position, ItemDataSO itemData = null)
    {
        if (itemData != null)
        {
            AssignData(itemData);
        }
        HideItem(false);
    }

    public void DespawnItem()
    {
        Destroy(gameObject);
    }

    private void AssignData(ItemDataSO itemData)
    {
        _itemData = itemData;
        GetComponent<MeshFilter>().mesh = _itemData.mesh;
        _meshRenderer.material = _itemData.meshMaterial;
    }

    private void HideItem(bool hideItem)
    {
        _meshRenderer.enabled = !hideItem;
    }
}
