using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ToolBarUI : ManagedByGameManager
{
    public InventoryPopupUI popupUI { get; private set; }

    public override void Init()
    {
        base.Init();
        popupUI = GetComponentInChildren<InventoryPopupUI>();
        if (popupUI == null)
        {
            throw new System.Exception("No Popup Found!");
        }
    }

    public override void CustomStart()
    {
        //SyncBarSize();
    }
}
