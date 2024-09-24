using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OxygenTank : MonoBehaviour
{
    [SerializeField] private OxygenTankSO _myData;
    public float oxygenLeft { get; private set; }
    public bool containsOxygen { get; private set; }
    private float _maxOxygenCapacity = 0;

    private void Awake()
    {
        _maxOxygenCapacity = _myData.minutesUntilEmpty * 60f;
        oxygenLeft = _maxOxygenCapacity;
        containsOxygen = (oxygenLeft > 0);
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
