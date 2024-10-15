using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class WI_OxygenTank : WorldItem
{
    [field :SerializeField] public OxygenTankSO oxygenTankData { get; protected set; }
    public float oxygenLeft
    {
        get
        {
            return _oxygenLeft;
        }
        protected set
        {
            _oxygenLeft = Mathf.Clamp(value, 0f, maxTankCapacity);
        }
    }
    protected float _oxygenLeft;
    public float maxTankCapacity;
    protected bool _initalized = false;

    protected override void Start()
    {
        base.Start();
        if (oxygenTankData != null)
        {
            Initialize(float.MaxValue, oxygenTankData);
        }
    }

    public void SpawnItem(Vector3 position, float oxygen, OxygenTankSO tankData = null)
    {
        SpawnItem(position, tankData);
        Initialize(oxygen, tankData);
    }

    public void Initialize(float oxygen, OxygenTankSO tankData)
    {
        if (_initalized)
        {
            return;
        }
        _initalized = true;
        if (tankData != null)
        {
            maxTankCapacity = tankData.minutesUntilEmpty * 60f;
        }
        oxygenLeft = oxygen;
    }

    public override void Interact()
    {
        GameManager.Instance.GetManagedComponent<InventoryController>().AddItemToInventory(oxygenTankData, oxygenLeft);
        DespawnItem();
    }
}
