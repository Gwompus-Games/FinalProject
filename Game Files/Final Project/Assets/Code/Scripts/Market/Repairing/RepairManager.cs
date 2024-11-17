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
        public void SetupValues(RepairTypes repairTypes, string name, int assignedBasePrice, int assignedScaleAdditive, float assignedMinPercentToBuy, Sprite buyButtonSprite)
        {
            type = repairTypes;
            nameString = name;
            basePrice = assignedBasePrice;
            currentPrice = basePrice;
            scaleAdditive = assignedScaleAdditive;
            minimumPercentToBuy = assignedMinPercentToBuy;
            buttonSprite = buyButtonSprite;
        }

        public void SetupValues(RepairValues values)
        {
            type = values.type;
            nameString = values.nameString;
            basePrice = values.basePrice;
            currentPrice = basePrice;
            scaleAdditive = values.scaleAdditive;
            minimumPercentToBuy = values.minimumPercentToBuy;
            buttonSprite = values.buttonSprite;
        }

        public RepairTypes type;
        public string nameString;
        public int basePrice;
        [HideInInspector]
        public int currentPrice;
        public int scaleAdditive;
        [Range(0f, 100f)] public float minimumPercentToBuy;
        public Sprite buttonSprite;
    }

    [SerializeField] private RepairValues[] _repairValues;
    [SerializeField] private GameObject _repairSectionPrefab;
    private Dictionary<RepairTypes, RepairValues> _repairs = new Dictionary<RepairTypes, RepairValues>();
    private SuitSystem _suitSystem;
    private RepairSection[] _repairSections;
    public bool debugMode = false;

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
        _suitSystem = GameManager.Instance.GetManagedComponent<SuitSystem>();
        for (int r = 0; r < _repairValues.Length; r++)
        {
#if UNITY_EDITOR
            if (_repairs.ContainsKey(_repairValues[r].type))
            {
                throw new Exception($"MORE THAN ONE {_repairValues[r].type.DisplayName()} FOUND!");
            }
#endif
            _repairValues[r].SetupValues(_repairValues[r]);
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
#if UNITY_EDITOR
            if (!_repairs.ContainsKey(repairTypes[r]))
            {
                throw new Exception($"{repairTypes[r].DisplayName()} REPAIR NOT ASSIGNED A VALUE!");
            }
#endif
        }
        return true;
    }

    private void SpawnSections()
    {
        List<RepairTypes> repairTypes = (Enum.GetValues(typeof(RepairTypes)) as RepairTypes[]).ToList();
        _repairSections = new RepairSection[repairTypes.Count];
        for (int t = 0; t < repairTypes.Count; t++)
        {
            _repairSections[t] = Instantiate(_repairSectionPrefab, transform).GetComponent<RepairSection>();
            _repairSections[t].InitilizeRepairSection(this, _repairs[repairTypes[t]]);
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
            default:
                RepairMade?.Invoke(repairCount);
                break;
        }

        _suitSystem.Repair(repairType);
    }

    public void UpdateAllSections()
    {
        for (int s = 0; s < _repairSections.Length; s++)
        {
            _repairSections[s].UpdateRepairSection();
        }
    }
}
