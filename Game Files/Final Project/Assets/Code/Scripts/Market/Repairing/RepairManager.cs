using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public class RepairManager : MonoBehaviour
{
    public enum RepairTypes
    {
        MINOR,
        MAJOR,
        REPLACEMENT
    }

    [Serializable]
    public struct RepairValues
    {
        public RepairValues(RepairTypes repairType = RepairTypes.MINOR)
        {
            initialized = false;
            type = repairType;
            nameString = string.Empty;
            basePrice = 0;
            currentPrice = 0;
            scaleAdditive = 0;
        }

        public void Initilize(RepairTypes repairTypes, string name, int assignedBasePrice, int assignedScaleAdditive)
        {
            initialized = true;
            type = repairTypes;
            nameString = name;
            basePrice = assignedBasePrice;
            currentPrice = basePrice;
            scaleAdditive = assignedScaleAdditive;
        }

        public void Initilize(RepairValues values)
        {
            initialized = true;
            type = values.type;
            nameString = values.nameString;
            basePrice = values.basePrice;
            currentPrice = values.currentPrice;
            scaleAdditive = values.scaleAdditive;
        }

        public bool initialized;
        public RepairTypes type;
        public string nameString;
        public int basePrice;
        public int currentPrice;
        public int scaleAdditive;
    }

    [SerializeField] private RepairValues[] _repairValues;
    [SerializeField] private GameObject _repairSectionPrefab;
    private Dictionary<RepairTypes, RepairValues> _repairs = new Dictionary<RepairTypes, RepairValues>();

    public int repairCount
    {
        get
        {
            return _rpc;
        }
        private set
        {
            _rpc = value;
            RepairMade?.Invoke(repairCount);
        }
    }
    public Action<int> RepairMade;
    private int _rpc;


    // Start is called before the first frame update
    void Start()
    {
        for (int r = 0; r < _repairValues.Length; r++)
        {
            if (_repairs.ContainsKey(_repairValues[r].type))
            {
                throw new Exception($"MORE THAN ONE {_repairValues[r].type.DisplayName()} FOUND!");
            }
            _repairValues[r].Initilize(_repairValues[r]);
            _repairs.Add(_repairValues[r].type, _repairValues[r]);
        }

        if (!CheckAllEntriesAreIncluded())
        {
            return;
        }

        SpawnSections();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private bool CheckAllEntriesAreIncluded()
    {
        List<RepairTypes> repairTypes = (Enum.GetValues(typeof(RepairTypes)) as RepairTypes[]).ToList();
        for (int r = 0; r < repairTypes.Count; r++)
        {
            if (!_repairs.ContainsKey(repairTypes[r]))
            {
                throw new Exception($"{repairTypes[r].DisplayName()} REPAIR NOT ASSIGNED A VALUE!");
                return false;
            }
        }
        return true;
    }

    private void SpawnSections()
    {
        List<RepairTypes> repairTypes = (Enum.GetValues(typeof(RepairTypes)) as RepairTypes[]).ToList();
        for (int t = 0; t < repairTypes.Count; t++)
        {
            RepairSection section = Instantiate(_repairSectionPrefab, transform).GetComponent<RepairSection>();
            section.InitilizeRepairSection(this, _repairs[repairTypes[t]]);
        }
    }

    public void RepairBought(RepairTypes repairType)
    {
        switch (repairType)
        {
            case RepairTypes.MAJOR:
                repairCount++;
                break;
            case RepairTypes.REPLACEMENT:
                repairCount = 0;
                break;
        }
    }
}
