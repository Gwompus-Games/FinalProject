using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class II_Glowstick : II_Tool
{
    private Color _chosenColour;
    

    protected override void Awake()
    {
        base.Awake();
        GlowstickSO glowstickSO = _holdableToolData as GlowstickSO;
        if (glowstickSO == null)
        {
            throw new System.Exception("Non glowstick data assigned to glowstick item!");
        }
        SetColour(glowstickSO.possibleColours[Random.Range(0, glowstickSO.possibleColours.Length)]);
    }

    public void SetColour(Color chosenColour)
    {
        _chosenColour = chosenColour;
        _itemImage.color = _chosenColour;
    }

    public Color GetColour()
    {
        return _chosenColour;
    }

    public override void ItemPlacedInInventory()
    {
        base.ItemPlacedInInventory();
        _toolController.AddGlowstickToQueue(this);
    }

    public override void ItemRemovedFromInventory()
    {
        base.ItemRemovedFromInventory();
        _toolController.RemoveGlowstickFromQueue(this);
    }
}
