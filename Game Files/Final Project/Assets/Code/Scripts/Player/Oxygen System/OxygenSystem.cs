using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class OxygenSystem : ManagedByGameManager
{
    public static Action<string, float> OxygenLeftInTank;

    public List<II_OxygenTank> oxygenTanks { get; private set; } = new List<II_OxygenTank>();
    public int activeOxygenTank { get; private set; } = 0;
    private List<OxygenDrainer> _oxygenDrainMultipliers = new List<OxygenDrainer>();
    [SerializeField] private OxygenTankSO _starterOxygenTank;
    [SerializeField] private int _numberOfStartingOxygenTanks;
    private PlayerController _playerController;
    private InventoryController _inventoryController;
    private GameManager.GameState currentGameState;
    [Header("Debug Settings")]
    [SerializeField] private bool _debugMode = false;

    public override void Init()
    {
        base.Init();
        _playerController = GameManager.Instance.GetManagedComponent<PlayerController>();
        _inventoryController = GameManager.Instance.GetManagedComponent<InventoryController>();
        currentGameState = GameManager.Instance.currentGameState;
    }

    public override void CustomStart()
    {
        base.CustomStart();
        if (_starterOxygenTank != null && _numberOfStartingOxygenTanks > 0)
        {
            for (int t = 0; t < _numberOfStartingOxygenTanks; t++)
            {
                _inventoryController.AddItemToInventory(_starterOxygenTank, float.MaxValue);
            }
        }
    }

    private void Update()
    {
        if (currentGameState != GameManager.GameState.LandedAtFacility)
        {
            return;
        }
        if (_playerController.onSub)
        {
            return;
        }
        
        DrainActiveTank(Time.deltaTime);
    }

    private void SortAndLabelOxygenTanks()
    {
        if (_debugMode)
        {
            Debug.Log($"Sorting oxygen tanks.");
        }
        switch (oxygenTanks.Count)
        {
            case < 1:
                if (_debugMode)
                {
                    Debug.Log($"No oxygen tanks found!");
                }
                break;
            case 1:
                if (_debugMode)
                {
                    Debug.Log($"Only one oxygen tank found!");
                }
                oxygenTanks[0].SetOxygenTankID(0);
                SwapOxygenTank();
                break;
            default:
                if (_debugMode)
                {
                    Debug.Log($"{oxygenTanks.Count} oxygen tanks found!");
                }
                oxygenTanks = SortOxygenTanks(oxygenTanks.ToArray());
                for (int t = 0; t < oxygenTanks.Count; t++)
                {
                    oxygenTanks[t].SetOxygenTankID(t);
                }
                break;
        }
    }

    private List<II_OxygenTank> SortOxygenTanks(II_OxygenTank[] oxygenTanks)
    {
        if (oxygenTanks.Length == 0)
        {
            Debug.LogWarning("Tried to sort 0 oxygen tanks!");
            return null;
        }
        if (oxygenTanks.Length == 1)
        {
            return new List<II_OxygenTank>(oxygenTanks);
        }
        List<II_OxygenTank> sortedOxygenTanks = new List<II_OxygenTank>();
        for (int ot = 0; ot < oxygenTanks.Length; ot++)
        {
            if (ot == 0)
            {
                sortedOxygenTanks.Add(oxygenTanks[ot]);
                continue;
            }
            if (!oxygenTanks[ot].containsOxygen)
            {
                sortedOxygenTanks.Add(oxygenTanks[ot]);
                continue;
            }
            bool foundSpot = false;
            for (int sot = 0; sot < sortedOxygenTanks.Count; sot++)
            {
                if (!sortedOxygenTanks[sot].containsOxygen)
                {
                    sortedOxygenTanks.Insert(sot, oxygenTanks[ot]);
                    foundSpot = true;
                    break;
                }
                if (sortedOxygenTanks[sot].oxygenLeft / sortedOxygenTanks[sot].maxOxygenCapacity < oxygenTanks[ot].oxygenLeft / oxygenTanks[ot].maxOxygenCapacity)
                {
                    sortedOxygenTanks.Insert(sot, oxygenTanks[ot]);
                    foundSpot = true;
                    break;
                }
            }
            if (!foundSpot)
            {
                sortedOxygenTanks.Add(oxygenTanks[ot]);
            }
        }
        return sortedOxygenTanks;
    }

    public void AddOxygenTank(II_OxygenTank tankToAdd)
    {
        if (oxygenTanks.Contains(tankToAdd))
        {
            return;
        }

        oxygenTanks.Add(tankToAdd);
        SortAndLabelOxygenTanks();
    }

    public void RemoveOxygenTank(II_OxygenTank tankToRemove)
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
            if (_debugMode)
            {
                Debug.Log($"No oxygen tanks in oxygen tanks list!");
            }
            return false;
        }
        if (!oxygenTanks[selectedTank].containsOxygen)
        {
            if (oxygenTanks.Count > 1)
            {
                for (int t = 1; t < oxygenTanks.Count; t++)
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
            else
            {
                if (_debugMode)
                {
                    Debug.Log($"Only one oxygen tank and it is empty!");
                }
                return false;
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
            Debug.LogWarning($"Swapping to {tank} tank, which has no oxygen.");
            return SwapOxygenTank();
        }

        activeOxygenTank = tank;
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

        if (oxygenTanks.Count == 0)
        {
            _playerController.NoOxygenLeft();
            OxygenLeftInTank?.Invoke("0", 0f);
            return;
        }

        while (drainAmountInSeconds > 0)
        {
            if (_debugMode)
            {
                Debug.Log($"Draining oxygen!");
            }
            oxygenTanks[activeOxygenTank].DrainOxygen(drainAmountInSeconds, out drainAmountInSeconds);
            if (drainAmountInSeconds > 0)
            {
                if (_debugMode)
                {
                    Debug.Log($"Roll over to the next tank!");
                }
                ReplaceOxygenTankWithEmpty(oxygenTanks[activeOxygenTank]);
                SortAndLabelOxygenTanks();
                if (!SwapOxygenTank())
                {
                    _playerController.NoOxygenLeft();
                    OxygenLeftInTank?.Invoke("0", 0f);
                    break;
                }
            }
        }

        if (drainAmountInSeconds == 0)
        {
            OxygenLeftInTank?.Invoke(oxygenTanks[activeOxygenTank].oxygenLeftPercent, 
                                     oxygenTanks[activeOxygenTank].oxygenFillAmount * 100f);
            _playerController.OxygenRegained();
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

    private void ReplaceOxygenTankWithEmpty(II_OxygenTank oxygenTank)
    {
        Vector2Int gridPosition = oxygenTank.originTile;
        ItemDataSO emptyOxygenTankData = oxygenTank.oxygenTankData.emptyTankData;
        _inventoryController.RemoveItemFromInventory(oxygenTank);
        _inventoryController.AddItemToInventory(emptyOxygenTankData, gridPosition);
    }

    public void UpdateGameState(GameManager.GameState newGameState)
    {
        currentGameState = newGameState;
    }

    private void OnEnable()
    {
        GameManager.UpdateGameState += UpdateGameState;
    }

    private void OnDisable()
    {
        GameManager.UpdateGameState -= UpdateGameState;        
    }
}
