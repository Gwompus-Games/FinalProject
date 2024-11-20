using System;
using UnityEngine;

public class SuitSystem : ManagedByGameManager, IDamageable
{
    public static Action<int, int, float, int> UpdateSuitUI;
    public int numberOfSections { get; private set; } = 5;
    public int currentSection { get; private set; } = 5;
    public float currentSectionDurability { get; private set; }

    public CameraShakeAndHitFeedback camShake;

    [field: SerializeField] public SuitStatsSO suitStats { get; private set; }
    [SerializeField] private float _damageToSuitPerSecond;

    public int maxSectionDurabitity => suitStats.maxDurabilityForSections;

    private int _sectionOnDive;

    private OxygenDrainer _suitOxygenDrainer;

    private PlayerController _playerController;

    public override void Init()
    {
        base.Init();
        _playerController = GameManager.Instance.GetManagedComponent<PlayerController>();
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

        _suitOxygenDrainer = (OxygenDrainer) gameObject.AddComponent(typeof(OxygenDrainer));
        _suitOxygenDrainer.SetDrainMultiplier(suitStats.oxygenDrainMultiplierForSections[currentSection]);
        _suitOxygenDrainer.ActivateDrainer();
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
        if (GameManager.Instance.currentGameState == GameManager.GameState.LandedAtFacility)
        {
            if (_playerController != null)
            {
                if (!_playerController.onSub)
                {
                    TakeDamage(_damageToSuitPerSecond * Time.deltaTime, false);
                }
            }
        }
#if UNITY_EDITOR
        if (Input.GetKeyDown(KeyCode.X))
        {
            DebugTakeDamage();
        }
#endif
    }

    public void DebugTakeDamage()
    {
        TakeDamage(maxSectionDurabitity / 4f);
    }

    public void TakeDamage(float damage)
    {
        TakeDamage(damage, true);
    }

    public void TakeDamage(float damage, bool screenShake)
    {
        while (damage >= currentSectionDurability)
        {
            DamageSection(damage, out damage);
        }
        currentSectionDurability -= damage;
        if (screenShake)
        {
            //Add player getting hit sound effect
            StartCoroutine(camShake.ShakeUrBooty(.15f, .4f));
        }
        UpdateUI();
    }

    private void DamageSection(float damage, out float remainderDamage)
    {
        if (currentSection >= numberOfSections - 1)
        {
            Debug.Log("Kill Player");
            GameManager.Instance.GetManagedComponent<PlayerController>().KillPlayer(DeathObject.DeathType.Beaten);
            remainderDamage = 0;
            return;
        }
        currentSection++;
        remainderDamage = Mathf.Max(damage - currentSectionDurability, 0);
        currentSectionDurability = suitStats.maxDurabilityForSections;
        int nextSectionToUse = Mathf.Min(currentSection, suitStats.oxygenDrainMultiplierForSections.Length - 1);
        _suitOxygenDrainer.SetDrainMultiplier(suitStats.oxygenDrainMultiplierForSections[nextSectionToUse]);
    }

    public void UpdateUI()
    {
        UpdateSuitUI?.Invoke(numberOfSections - currentSection,
                             numberOfSections,
                             currentSectionDurability,
                             maxSectionDurabitity);
    }

    public void UpdateSectionOnDive()
    {
        _sectionOnDive = currentSection;
    }

    public void ResetSuitDurability()
    {
        currentSection = _sectionOnDive + 1;
        if (currentSection >= numberOfSections)
        {
            currentSection = numberOfSections - 1;
        }
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
