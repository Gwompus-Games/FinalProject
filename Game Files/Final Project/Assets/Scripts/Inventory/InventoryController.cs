using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventoryController : MonoBehaviour
{
    [HideInInspector]
    public InventoryGrid selectedItemGrid;

    private void Update()
    {
        if (selectedItemGrid == null)
        {
            return;
        }
        if (!selectedItemGrid.gameObject.activeInHierarchy)
        {
            return;
        }

        //Debug.Log(selectedItemGrid.GetTileGridPosition(Input.mousePosition));
        if (Input.GetMouseButtonDown(0))
        {
            Debug.Log(selectedItemGrid.GetTileGridPosition(Input.mousePosition));
        }
    }
}
