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
        public RepairTypes type;
        public string nameString;
        public int basePrice;
        public int currentPrice;
        public int scaleAdditive;
    }

    [SerializeField] private RepairValues[] _repairValues;
    [SerializeField] private GameObject _repairSectionPrefab;
    private Dictionary<RepairTypes, RepairValues> _repairs = new Dictionary<RepairTypes, RepairValues>();

    // Start is called before the first frame update
    void Start()
    {
        for (int r = 0; r < _repairValues.Length; r++)
        {
            if (_repairs.ContainsKey(_repairValues[r].type))
            {
                throw new Exception($"MORE THAN ONE {_repairValues[r].type.DisplayName()} FOUND!");
            }
            _repairs.Add(_repairValues[r].type, _repairValues[r]);
        }

        if (!CheckAllEntriesAreIncluded())
        {
            return;
        }


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
}
