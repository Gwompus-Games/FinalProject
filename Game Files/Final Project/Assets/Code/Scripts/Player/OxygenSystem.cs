using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class OxygenSystem : MonoBehaviour
{
    public static OxygenSystem INSTANCE;

    [SerializeField] private List<OxygenTank> oxygenTanks = new List<OxygenTank>();
    private int activeOxygenTank = 0;
    private Dictionary<MonoBehaviour, float> oxygenDrainMultipliers = new Dictionary<MonoBehaviour, float>();

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

    private void SwapOxygenTank()
    {
        int selectedTank = 0;
        if (oxygenTanks.Count < 1)
        {
            return;
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
            }
        }

        activeOxygenTank = selectedTank;
    }

    private void SwapOxygenTank(int tank)
    {
        if (tank >= oxygenTanks.Count || tank < 0)
        {
            throw new System.Exception($"Tried to swap to {tank} tank, which is out of range.");
        }

        if (!oxygenTanks[tank].containsOxygen)
        {
            Debug.Log($"Swapping to {tank} tank, which has no oxygen.");
        }

        activeOxygenTank = tank;
    }
}
