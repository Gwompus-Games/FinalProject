using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "SuitStats", menuName = "Stats/Suit Stats")]
public class SuitStatsSO : ScriptableObject
{
    public int numberOfSections = 5;
    public int maxDurabilityForSections;
    public float[] oxygenDrainMultiplierForSections;
    public float numberOfMinutesForSectionDurability = 15f;
}
