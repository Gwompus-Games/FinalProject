using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OxygenTank : InventoryItem
{
    [SerializeField] private OxygenTankSO _myData;
    public float oxygenLeft { get; private set; }
    public bool containsOxygen { get; private set; }
    private float _maxOxygenCapacity = 0;
    private int _myOxygenTankID = 0;
    public string oxygenLeftPercent { get; private set; }

    private void Awake()
    {
        _maxOxygenCapacity = _myData.minutesUntilEmpty * 60f;
        oxygenLeft = _maxOxygenCapacity;
        containsOxygen = (oxygenLeft > 0);
        oxygenLeftPercent = "100";
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
        if (oxygenLeft <= 0)
        {
            containsOxygen = false;
        }
        oxygenLeftPercent = string.Format("{0:0.##}", oxygenLeft / _maxOxygenCapacity * 100);
    }

    public void AddOxygen(float oxygenToAddInSeconds, out float remainder)
    {
        remainder = 0;
        containsOxygen = true;
        if (oxygenToAddInSeconds >= _maxOxygenCapacity)
        {
            SetTankToFull();
            remainder = oxygenToAddInSeconds - _maxOxygenCapacity;
            return;
        }

        if (oxygenLeft + oxygenToAddInSeconds >= _maxOxygenCapacity)
        {
            SetTankToFull();
            remainder = oxygenLeft + oxygenToAddInSeconds - _maxOxygenCapacity;
            return;
        }

        oxygenLeft = Mathf.Min(oxygenLeft + oxygenToAddInSeconds, _maxOxygenCapacity);
    }

    public void SetTankToFull()
    {
        oxygenLeft = _maxOxygenCapacity;
        containsOxygen = true;
    }
}
