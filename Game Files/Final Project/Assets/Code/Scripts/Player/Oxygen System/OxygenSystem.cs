using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class OxygenSystem : MonoBehaviour
{
    public static OxygenSystem INSTANCE;
    public static Action<string> OxygenLeftInTank;

    [SerializeField] private List<OxygenTank> oxygenTanks = new List<OxygenTank>();
    private int activeOxygenTank = 0;
    private List<OxygenDrainer> oxygenDrainMultipliers = new List<OxygenDrainer>();

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
        
    }

    private void Update()
    {
        DrainActiveTank(Time.deltaTime);
    }

    private void SortAndLabelOxygenTanks()
    {
        switch (oxygenTanks.Count)
        {
            case < 1:
                break;
            case 1:
                oxygenTanks[0].SetOxygenTankID(0);
                SwapOxygenTank(0);
                break;
            default:
                for (int t = 0; t < oxygenTanks.Count; t++)
                {
                    oxygenTanks[t].SetOxygenTankID(t);
                }
                break;
        }
    }

    public void AddOxygenTank(OxygenTank tankToAdd)
    {
        if (oxygenTanks.Contains(tankToAdd))
        {
            return;
        }

        oxygenTanks.Add(tankToAdd);
        SortAndLabelOxygenTanks();
    }

    public void RemoveOxygenTank(OxygenTank tankToRemove)
    {
        if (!oxygenTanks.Contains(tankToRemove))
        {
            return;
        }

        oxygenTanks.Remove(tankToRemove);
        SortAndLabelOxygenTanks();
    }

    public void RemoveOxygenTank(int tankIDToRemove)
    {
        if (tankIDToRemove < 0 || tankIDToRemove >= oxygenTanks.Count)
        {
            Debug.LogWarning($"Tried to remove tank ID {tankIDToRemove}, which is outside the scope of the List");
            return;
        }

        oxygenTanks.RemoveAt(tankIDToRemove);
        SortAndLabelOxygenTanks();
    }

    private bool SwapOxygenTank()
    {
        int selectedTank = 0;
        if (oxygenTanks.Count < 1)
        {
            return false;
        }
        if (!oxygenTanks[selectedTank].containsOxygen)
        {
            for (int t = 0; t < oxygenTanks.Count; t++)
            {
                if (oxygenTanks[t].containsOxygen)
                {
                    selectedTank = t;
                    break;
                }
                if (t >= oxygenTanks.Count - 1)
                {
                    return false;
                }
            }
        }

        activeOxygenTank = selectedTank;
        return true;
    }

    private bool SwapOxygenTank(int tank)
    {
        if (tank >= oxygenTanks.Count || tank < 0)
        {
            throw new System.Exception($"Tried to swap to {tank} tank, which is out of range.");
        }

        if (!oxygenTanks[tank].containsOxygen)
        {
            Debug.Log($"Swapping to {tank} tank, which has no oxygen.");
            return SwapOxygenTank();
        }

        activeOxygenTank = tank;
        return true;
    }

    public void AddDrainingSource(OxygenDrainer drainer)
    {
        if (oxygenDrainMultipliers.Contains(drainer))
        {
            Debug.Log($"List of drainers already contains {drainer.name}");
            return;
        }

        oxygenDrainMultipliers.Add(drainer);
    }

    public void RemoveDrainingSource(OxygenDrainer drainer)
    {
        if (!oxygenDrainMultipliers.Contains(drainer))
        {
            Debug.Log($"List of drainers already contains {drainer.name}");
            return;
        }

        oxygenDrainMultipliers.Remove(drainer);
    }

    public bool DrainingSourceActive(OxygenDrainer drainer)
    {
        return oxygenDrainMultipliers.Contains(drainer);
    }

    private void DrainActiveTank(float drainAmountInSeconds)
    {
        drainAmountInSeconds = ApplyDrainModifiers(drainAmountInSeconds);

        if (oxygenTanks.Count <= 0)
        {
            PlayerController.INSTANCE.NoOxygenLeft();
            return;
        }

        while (drainAmountInSeconds > 0)
        {
            oxygenTanks[activeOxygenTank].DrainOxygen(drainAmountInSeconds, out drainAmountInSeconds);
            if (drainAmountInSeconds >= 0)
            {
                if (!SwapOxygenTank())
                {
                    PlayerController.INSTANCE.NoOxygenLeft();
                    OxygenLeftInTank?.Invoke("0");
                    break;
                }
            }
        }

        if (drainAmountInSeconds <= 0)
        {
            OxygenLeftInTank?.Invoke(oxygenTanks[activeOxygenTank].oxygenLeftPercent);
        }
    }

    private float ApplyDrainModifiers(float baseDrainAmountInSeconds)
    {
        if (oxygenDrainMultipliers.Count <= 0)
        {
            return baseDrainAmountInSeconds;
        }
        float modifiedDrainAmountInSeconds = baseDrainAmountInSeconds;

        for (int d = 0; d < oxygenDrainMultipliers.Count; d++)
        {
            modifiedDrainAmountInSeconds *= oxygenDrainMultipliers[d].drainMultiplier;
        }
        return modifiedDrainAmountInSeconds;
    }
}
