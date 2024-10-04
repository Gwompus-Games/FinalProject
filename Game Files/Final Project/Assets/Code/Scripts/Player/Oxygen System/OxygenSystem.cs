using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class OxygenSystem : MonoBehaviour
{
    public static OxygenSystem INSTANCE;
    public static Action<string, float> OxygenLeftInTank;

    [SerializeField] private List<II_OxygenTank> _oxygenTanks = new List<II_OxygenTank>();
    private int _activeOxygenTank = 0;
    private List<OxygenDrainer> _oxygenDrainMultipliers = new List<OxygenDrainer>();
    [SerializeField] private OxygenTankSO _starterOxygenTank;
    [SerializeField] private int _numberOfStartingOxygenTanks;

    private void Awake()
    {
        if (INSTANCE != null)
        {
            Destroy(gameObject);
            return;
        }
        INSTANCE = this;
    }

    private void Start()
    {
        if (_starterOxygenTank != null && _numberOfStartingOxygenTanks > 0)
        {
            for (int t = 0; t < _numberOfStartingOxygenTanks; t++)
            {
                InventoryController.INSTANCE.AddItemToInventory(_starterOxygenTank);
            }
        }
    }

    private void Update()
    {
        DrainActiveTank(Time.deltaTime);
    }

    private void SortAndLabelOxygenTanks()
    {
        switch (_oxygenTanks.Count)
        {
            case < 1:
                break;
            case 1:
                _oxygenTanks[0].SetOxygenTankID(0);
                SwapOxygenTank(0);
                break;
            default:
                for (int t = 0; t < _oxygenTanks.Count; t++)
                {
                    _oxygenTanks[t].SetOxygenTankID(t);
                }
                break;
        }
    }

    public void AddOxygenTank(II_OxygenTank tankToAdd)
    {
        if (_oxygenTanks.Contains(tankToAdd))
        {
            return;
        }

        _oxygenTanks.Add(tankToAdd);
        SortAndLabelOxygenTanks();
    }

    public void RemoveOxygenTank(II_OxygenTank tankToRemove)
    {
        if (!_oxygenTanks.Contains(tankToRemove))
        {
            return;
        }

        _oxygenTanks.Remove(tankToRemove);
        SortAndLabelOxygenTanks();
    }

    public void RemoveOxygenTank(int tankIDToRemove)
    {
        if (tankIDToRemove < 0 || tankIDToRemove >= _oxygenTanks.Count)
        {
            Debug.LogWarning($"Tried to remove tank ID {tankIDToRemove}, which is outside the scope of the List");
            return;
        }

        _oxygenTanks.RemoveAt(tankIDToRemove);
        SortAndLabelOxygenTanks();
    }

    private bool SwapOxygenTank()
    {
        int selectedTank = 0;
        if (_oxygenTanks.Count < 1)
        {
            return false;
        }
        if (!_oxygenTanks[selectedTank].containsOxygen)
        {
            for (int t = 0; t < _oxygenTanks.Count; t++)
            {
                if (_oxygenTanks[t].containsOxygen)
                {
                    selectedTank = t;
                    break;
                }
                if (t >= _oxygenTanks.Count - 1)
                {
                    return false;
                }
            }
        }

        _activeOxygenTank = selectedTank;
        return true;
    }

    private bool SwapOxygenTank(int tank)
    {
        if (tank >= _oxygenTanks.Count || tank < 0)
        {
            throw new System.Exception($"Tried to swap to {tank} tank, which is out of range.");
        }

        if (!_oxygenTanks[tank].containsOxygen)
        {
            Debug.Log($"Swapping to {tank} tank, which has no oxygen.");
            return SwapOxygenTank();
        }

        _activeOxygenTank = tank;
        return true;
    }

    public void AddDrainingSource(OxygenDrainer drainer)
    {
        if (_oxygenDrainMultipliers.Contains(drainer))
        {
            Debug.Log($"List of drainers already contains {drainer.name}");
            return;
        }

        _oxygenDrainMultipliers.Add(drainer);
    }

    public void RemoveDrainingSource(OxygenDrainer drainer)
    {
        if (!_oxygenDrainMultipliers.Contains(drainer))
        {
            Debug.Log($"List of drainers already contains {drainer.name}");
            return;
        }

        _oxygenDrainMultipliers.Remove(drainer);
    }

    public bool DrainingSourceActive(OxygenDrainer drainer)
    {
        return _oxygenDrainMultipliers.Contains(drainer);
    }

    private void DrainActiveTank(float drainAmountInSeconds)
    {
        drainAmountInSeconds = ApplyDrainModifiers(drainAmountInSeconds);

        if (_oxygenTanks.Count == 0)
        {
            PlayerController.Instance.NoOxygenLeft();
            OxygenLeftInTank?.Invoke("0", 0f);
            return;
        }

        while (drainAmountInSeconds > 0)
        {
            _oxygenTanks[_activeOxygenTank].DrainOxygen(drainAmountInSeconds, out drainAmountInSeconds);
            if (drainAmountInSeconds > 0)
            {
                if (!SwapOxygenTank())
                {
                    PlayerController.Instance.NoOxygenLeft();
                    OxygenLeftInTank?.Invoke("0", 0f);
                    break;
                }
            }
        }

        if (drainAmountInSeconds == 0)
        {
            OxygenLeftInTank?.Invoke(_oxygenTanks[_activeOxygenTank].oxygenLeftPercent, _oxygenTanks[_activeOxygenTank].oxygenLeft / _oxygenTanks[_activeOxygenTank].maxOxygenCapacity * 100f);
        }
    }

    private float ApplyDrainModifiers(float baseDrainAmountInSeconds)
    {
        if (_oxygenDrainMultipliers.Count == 0)
        {
            return baseDrainAmountInSeconds;
        }
        float modifiedDrainAmountInSeconds = baseDrainAmountInSeconds;

        for (int d = 0; d < _oxygenDrainMultipliers.Count; d++)
        {
            modifiedDrainAmountInSeconds *= _oxygenDrainMultipliers[d].drainMultiplier;
        }
        return modifiedDrainAmountInSeconds;
    }
}
