using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class II_OxygenTank : InventoryItem
{
    public OxygenTankSO oxygenTankData { get; protected set; }
    public float oxygenLeft { 
        get
        {
            return _oxygenLeft;
        }
        protected set
        {
            _oxygenLeft = Mathf.Clamp(value, 0f, maxOxygenCapacity);
            oxygenFillAmount = oxygenLeft / maxOxygenCapacity;
            oxygenLeftPercent = string.Format("{0:0.##}", oxygenFillAmount * 100);
        }
    }
    protected float _oxygenLeft;
    public bool containsOxygen => oxygenLeft > 0;
    public float maxOxygenCapacity { get; protected set; } = 0;
    protected int _myOxygenTankID = 0;
    public string oxygenLeftPercent { get; protected set; }
    public float oxygenFillAmount { get; protected set; }
    private OxygenSystem _oxygenSystem;


    protected override void Awake()
    {
        base.Awake();
        oxygenTankData = itemData as OxygenTankSO;
        if (oxygenTankData == null)
        {
            throw new System.Exception("Item data not set to an Oxygen Tank Data!");
        }
        _oxygenSystem = GameManager.Instance.GetManagedComponent<OxygenSystem>();
        //maxOxygenCapacity = oxygenTankData.minutesUntilEmpty * 60f;
        //SetTankToFull();
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
        if (drainAmountInSeconds > oxygenLeft)
        {
            remainder = drainAmountInSeconds - oxygenLeft;
            oxygenLeft = 0;
            return;
        }
        oxygenLeft -= drainAmountInSeconds;
        remainder = 0;
    }

    public void AddOxygen(float oxygenToAddInSeconds, out float remainder)
    {
        remainder = 0;
        if (oxygenLeft + oxygenToAddInSeconds >= maxOxygenCapacity)
        {
            SetTankToFull();
            remainder = oxygenLeft + oxygenToAddInSeconds - maxOxygenCapacity;
            return;
        }

        oxygenLeft += oxygenToAddInSeconds;
    }

    public void SetTankOxygenLevel(float oxygen)
    {
        oxygenLeft = oxygen;
    }

    public void SetTankToFull()
    {
        oxygenLeft = maxOxygenCapacity;
    }

    public void InitializeTank(OxygenTankSO tankData)
    {
        if (tankData == null)
        {
            Debug.Log("Null data passed");
            return;
        }
        oxygenTankData = tankData;
        maxOxygenCapacity = oxygenTankData.minutesUntilEmpty * 60f;
    }

    public override void ItemPlacedInInventory()
    {
        base.ItemPlacedInInventory();
        _oxygenSystem.AddOxygenTank(this);
    }

    public override void ItemRemovedFromInventory()
    {
        base.ItemRemovedFromInventory();
        _oxygenSystem.RemoveOxygenTank(this);
    }
}
