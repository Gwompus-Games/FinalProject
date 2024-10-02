using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OxygenTank : InventoryItem
{
    [SerializeField] private OxygenTankSO _oxygenTankData;
    public float oxygenLeft { 
        get
        {
            return _oxygenLeft;
        }
        private set
        {
            _oxygenLeft = value;
            oxygenLeftPercent = string.Format("{0:0.##}", oxygenLeft / maxOxygenCapacity * 100);
        }
    }
    public float _oxygenLeft { get; private set; }
    public bool containsOxygen { get; private set; }
    public float maxOxygenCapacity { get; private set; } = 0;
    private int _myOxygenTankID = 0;
    public string oxygenLeftPercent { get; private set; }

    private void Awake()
    {
        maxOxygenCapacity = _oxygenTankData.minutesUntilEmpty * 60f;
        oxygenLeft = maxOxygenCapacity;
        containsOxygen = (oxygenLeft > 0);
    }

    public void SetOxygenTankID(int id)
    {
        if (id < 0)
        {
            throw new System.Exception("Assigning oxygen tank ID to a negative number!");
        }
        _myOxygenTankID = id;
    }

    public int GetOxygenTankID()
    {
        return _myOxygenTankID;
    }

    public void DrainOxygen(float drainAmountInSeconds, out float remainder)
    {
        oxygenLeft -= drainAmountInSeconds;
        remainder = 0;
        if (oxygenLeft < 0)
        {
            remainder = -oxygenLeft;
            oxygenLeft = 0;
        }
        if (oxygenLeft == 0)
        {
            containsOxygen = false;
        }
    }

    public void AddOxygen(float oxygenToAddInSeconds, out float remainder)
    {
        remainder = 0;
        containsOxygen = true;
        if (oxygenToAddInSeconds >= maxOxygenCapacity)
        {
            SetTankToFull();
            remainder = oxygenToAddInSeconds - maxOxygenCapacity;
            return;
        }

        if (oxygenLeft + oxygenToAddInSeconds >= maxOxygenCapacity)
        {
            SetTankToFull();
            remainder = oxygenLeft + oxygenToAddInSeconds - maxOxygenCapacity;
            return;
        }

        oxygenLeft = Mathf.Min(oxygenLeft + oxygenToAddInSeconds, maxOxygenCapacity);
    }

    public void SetTankToFull()
    {
        oxygenLeft = maxOxygenCapacity;
        containsOxygen = true;
    }

    public override void ItemPlacedInInventory()
    {
        base.ItemPlacedInInventory();
        OxygenSystem.INSTANCE.AddOxygenTank(this);
    }

    public override void ItemRemovedFromInventory()
    {
        base.ItemRemovedFromInventory();
        OxygenSystem.INSTANCE.RemoveOxygenTank(this);
    }
}
