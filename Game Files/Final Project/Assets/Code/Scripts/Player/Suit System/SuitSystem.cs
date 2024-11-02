using System;
using UnityEngine;

public class SuitSystem : ManagedByGameManager, IDamageable
{
    public static Action<int, int, float, int> UpdateSuitUI;
    public int numberOfSections { get; private set; } = 5;
    public int currentSection { get; private set; } = 5;
    public float currentSectionDurability { get; private set; }

    [field: SerializeField] public SuitStatsSO suitStats { get; private set; }

    public int maxSectionDurabitity => suitStats.maxDurabilityForSections;

    private OxygenDrainer suitOxygenDrainer;

    public override void Init()
    {
        base.Init();
    }

    public override void CustomStart()
    {
        base.CustomStart();
        if (suitStats == null)
        {
            throw new Exception("No Suit Stats added to suit system!");
        }

        numberOfSections = suitStats.numberOfSections;
        currentSection = 0;
        currentSectionDurability = suitStats.maxDurabilityForSections;

        suitOxygenDrainer = (OxygenDrainer) gameObject.AddComponent(typeof(OxygenDrainer));
        suitOxygenDrainer.SetDrainMultiplier(suitStats.oxygenDrainMultiplierForSections[currentSection]);
        suitOxygenDrainer.ActivateDrainer();
        UpdateUI();
    }

    private void OnEnable()
    {
        //CustomPlayerInput.OpenInventory += DebugTakeDamage;
    }

    private void OnDisable()
    {
        //CustomPlayerInput.OpenInventory -= DebugTakeDamage;        
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.X))
        {
            DebugTakeDamage();
        }
    }

    public void DebugTakeDamage()
    {
        TakeDamage(maxSectionDurabitity / 4f);
    }

    public void TakeDamage(float damage)
    {
        while (damage >= currentSectionDurability)
        {
            DamageSection(damage, out damage);
        }
        currentSectionDurability -= damage;
        UpdateUI();
    }

    private void DamageSection(float damage, out float remainderDamage)
    {
        if (currentSection >= numberOfSections - 1)
        {
            Debug.Log("Kill Player");
            GameManager.Instance.GetManagedComponent<PlayerController>().KillPlayer(ParentDeath.DeathType.Beaten);
            remainderDamage = 0;
            return;
        }
        currentSection++;
        remainderDamage = Mathf.Max(damage - currentSectionDurability, 0);
        currentSectionDurability = suitStats.maxDurabilityForSections;
        int nextSectionToUse = Mathf.Min(currentSection, suitStats.oxygenDrainMultiplierForSections.Length - 1);
        suitOxygenDrainer.SetDrainMultiplier(suitStats.oxygenDrainMultiplierForSections[nextSectionToUse]);
    }

    public void UpdateUI()
    {
        UpdateSuitUI?.Invoke(numberOfSections - currentSection,
                             numberOfSections,
                             currentSectionDurability,
                             maxSectionDurabitity);
    }

    public void Repair(RepairManager.RepairTypes repairType)
    {
        currentSectionDurability = maxSectionDurabitity;
        switch (repairType)
        {
            case RepairManager.RepairTypes.MINOR:
                break;
            default:
                currentSection = 0;
                break;
        }
        UpdateUI();
    }
}
