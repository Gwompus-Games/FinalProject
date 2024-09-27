using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(MeshRenderer))]
public class Item : MonoBehaviour
{
    protected GameObject _worldObject;
    protected GameObject _inventoryObject;

    public virtual void AddToInventory()
    {
        _worldObject.SetActive(false);
        _inventoryObject.SetActive(true);
    }

    public virtual void RemoveFromInventory()
    {
        PlaceObjectInFrontOfPlayer();
        _worldObject.SetActive(true);
        _inventoryObject.SetActive(false);
    }

    protected void PlaceObjectInFrontOfPlayer()
    {
        Transform playerTransform = PlayerController.INSTANCE.transform;
        _worldObject.transform.position = playerTransform.position + playerTransform.forward;
    }
}
