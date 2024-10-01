using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TreasureSpawnPoint : MonoBehaviour
{
    [SerializeField] private TreasureListSO _treasureList;
    [SerializeField] private bool _useWeightedSpawning;
    private bool _treasureSpawned = false;
    public static Dictionary<TreasureSO.TreasureRarity, List<TreasureSO>> treasuresByRarity { get; private set; } = new Dictionary<TreasureSO.TreasureRarity, List<TreasureSO>>();
    public static List<TreasureSO> uniqueTreasuresSpawned { get; private set; } = new List<TreasureSO>();

    private void Awake()
    {
        if (treasuresByRarity.Count == 0)
        {
            treasuresByRarity.Add(TreasureSO.TreasureRarity.COMMON, new List<TreasureSO>());
            treasuresByRarity.Add(TreasureSO.TreasureRarity.UNCOMMON, new List<TreasureSO>());
            treasuresByRarity.Add(TreasureSO.TreasureRarity.RARE, new List<TreasureSO>());
            treasuresByRarity.Add(TreasureSO.TreasureRarity.UNIQUE, new List<TreasureSO>());
            for (int t = 0; t < _treasureList.treasures.Length; t++)
            {
                treasuresByRarity[_treasureList.treasures[t].rarity].Add(_treasureList.treasures[t]);
            }
        }
    }

    public void SpawnTreasureByRarity(params TreasureSO.TreasureRarity[] rarity)
    {
        if (_treasureSpawned)
        {
            return;
        }
        TreasureSO[] validTreasures;
        switch (rarity.Length)
        {
            case 0:
                return;
            case 1:
                validTreasures = treasuresByRarity[rarity[0]].ToArray();
                break;
            default:
                List<TreasureSO> treasures = new List<TreasureSO>();
                for (int r = 0; r < rarity.Length; r++)
                {
                    treasures.AddRange(treasuresByRarity[rarity[r]]);
                    if (rarity[r] == TreasureSO.TreasureRarity.UNIQUE)
                    {
                        for (int ut = 0; ut < uniqueTreasuresSpawned.Count; ut++)
                        {
                            if (treasures.Contains(uniqueTreasuresSpawned[ut]))
                            {
                                treasures.Remove(uniqueTreasuresSpawned[ut]);
                            }
                        }
                    }
                }
                validTreasures = treasures.ToArray();
                break;

        }
        int treasurePicked = 0;
        if (_useWeightedSpawning)
        {
            int chanceTotal = 0;
            for (int t = 0; t < validTreasures.Length; t++)
            {
                chanceTotal += validTreasures[t].spawnChancePoints;
            }
            int chancePicked = Random.Range(0, chanceTotal);
            for (int t = 0; t < validTreasures.Length; t++)
            {
                chancePicked -= validTreasures[t].spawnChancePoints;
                if (chancePicked <= 0)
                {
                    treasurePicked = t;
                    break;
                }
            }
        }
        else
        {
            treasurePicked = Random.Range(0, validTreasures.Length);
        }
        SpawnTreasure(validTreasures[treasurePicked]);
    }

    public void SpawnSpecificTreasure(TreasureSO treasureToSpawn)
    {
        if (_treasureSpawned)
        {
            return;
        }
        SpawnTreasure(treasureToSpawn);
    }

    private void SpawnTreasure(TreasureSO treasureToSpawn)
    {
        if (_treasureSpawned)
        {
            return;
        }
        if (treasureToSpawn.rarity == TreasureSO.TreasureRarity.UNIQUE)
        {
            uniqueTreasuresSpawned.Add(treasureToSpawn);
        }
        GameObject treasureGO = Instantiate(treasureToSpawn.worldObject, transform.position, Quaternion.identity);
        treasureGO.transform.parent = FindObjectOfType<WorldItemsTag>().transform;
        _treasureSpawned = true;
    }
}
