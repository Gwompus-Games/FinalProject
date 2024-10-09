using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class II_OxygenTank : InventoryItem
{
    [SerializeField] protected OxygenTankSO _oxygenTankData;
    public float oxygenLeft { 
        get
        {
            return _oxygenLeft;
        }
        protected set
        {
            _oxygenLeft = value;
            oxygenLeftPercent = string.Format("{0:0.##}", oxygenLeft / maxOxygenCapacity * 100);
        }
    }
    private float _oxygenLeft;
    public bool containsOxygen => oxygenLeft > 0;
    public float maxOxygenCapacity { get; private set; } = 0;
    protected int _myOxygenTankID = 0;
    public string oxygenLeftPercent { get; private set; }

    protected override void Awake()
    {
        base.Awake();
        maxOxygenCapacity = _oxygenTankData.minutesUntilEmpty * 60f;
        oxygenLeft = maxOxygenCapacity;
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
    }

    public void AddOxygen(float oxygenToAddInSeconds, out float remainder)
    {
        remainder = 0;
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
    }

    public override void ItemPlacedInInventory()
    {
        base.ItemPlacedInInventory();
        GameManager.OxygenSystemInstance.AddOxygenTank(this);
    }

    public override void ItemRemovedFromInventory()
    {
        base.ItemRemovedFromInventory();
        GameManager.OxygenSystemInstance.RemoveOxygenTank(this);
    }
}
