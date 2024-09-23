using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SuitSystem : MonoBehaviour
{
    public static SuitSystem INSTANCE;
    public static Action UpdateSuitUI;
    [field: SerializeField] public int numberOfSections { get; private set; } = 5;
    [field: SerializeField] public float currentSection { get; private set; } = 5;
    [SerializeField] private float[] oxygenDrainForSection;

    [field: SerializeField] public float suitDurabilitySectionMax { get; private set; } = 100;
    [field: SerializeField] public float suitDurabilityForCurrentSection { get; private set; } = 100;
    

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
        UpdateSuitUI?.Invoke();
    }

    private void OnEnable()
    {
        CustomPlayerInput.OpenInventory += DebugTakeDamage;
    }

    private void OnDisable()
    {
        CustomPlayerInput.OpenInventory -= DebugTakeDamage;        
    }

    private void Update()
    {
        
    }

    public void DebugTakeDamage()
    {
        TakeSuitDamage(75f);
    }

    public void TakeSuitDamage(float damage)
    {
        while (damage >= suitDurabilityForCurrentSection)
        {
            DamageSection(damage, out damage);
        }
        suitDurabilityForCurrentSection -= damage;
        UpdateSuitUI?.Invoke();
    }

    private void DamageSection(float damage, out float remainderDamage)
    {
        if (currentSection <= 1)
        {
            Debug.Log("Kill Player");
        }
        currentSection--;
        remainderDamage = Mathf.Max(damage - suitDurabilityForCurrentSection, 0);
        suitDurabilityForCurrentSection = 100f;
    }
}
