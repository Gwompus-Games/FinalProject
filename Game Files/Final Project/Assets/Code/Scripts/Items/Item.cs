using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(MeshRenderer))]
public class Item : MonoBehaviour
{
    private Image _inventoryImage;
    private MeshRenderer _meshRenderer;

    protected virtual void Awake()
    {
        _meshRenderer = GetComponent<MeshRenderer>();
        _inventoryImage = GetComponentInChildren<Image>();

    }

    public virtual void AddToInventory()
    {

    }

    public virtual void RemoveFromInventory()
    {

    }
}
