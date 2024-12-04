using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TreasureSpawnPoint : MonoBehaviour
{
    [Header("Treasure spawning options:\nLeave empty if you want to spawn from all rarities other than unique!")]
    [Tooltip("Adding the unique treasure to this list will NOT spawn from unique treasures!\nPlease use the boolean underneath for unique treasures!")]
    [SerializeField] private TreasureSO.TreasureRarity[] _treasureRaritiesToChooseFrom;
    [Tooltip("Use this boolean for toggling if you want to be able to spawn unique treasures!")]
    [SerializeField] private bool _ableToSpawnUniqueTreasures;

    [Header("LEAVE EMPTY UNLESS YOU KNOW WHAT YOU ARE DOING!!!!\nAdding any treasure list here will override spawning options above!")]
    [Tooltip("Leave empty if you want to spawn a random treasure from the master treasure list!\nOnly use if you want a specific treasure to spawn or spawn randomly from a couple of specific treasures!")]
    [SerializeField] private TreasureListSO _manualSpawnTreasuresList;
    
    [Header("Do not change these variables!")]
    [Tooltip("Assign the master treasure list here!\nDo not change this variable under any circumstances!")]
    [SerializeField] private MasterTreasureListSO _masterTreasureList;

    public static MasterTreasureListSO staticMasterTreasureList;
    private bool _treasureSpawned = false;
    public static Dictionary<TreasureSO.TreasureRarity, List<TreasureSO>> treasuresByRarity { get; private set; } = new Dictionary<TreasureSO.TreasureRarity, List<TreasureSO>>();
    public static List<TreasureSO> uniqueTreasuresSpawned { get; private set; } = new List<TreasureSO>();

    private void Awake()
    {
        if (staticMasterTreasureList == null && _masterTreasureList != null)
        {
            staticMasterTreasureList = _masterTreasureList;
        }
        else if (staticMasterTreasureList != null)
        {
            _masterTreasureList = staticMasterTreasureList;
        }
        if (treasuresByRarity.Count == 0)
        {
            treasuresByRarity.Add(TreasureSO.TreasureRarity.COMMON, new List<TreasureSO>());
            treasuresByRarity.Add(TreasureSO.TreasureRarity.UNCOMMON, new List<TreasureSO>());
            treasuresByRarity.Add(TreasureSO.TreasureRarity.RARE, new List<TreasureSO>());
            treasuresByRarity.Add(TreasureSO.TreasureRarity.UNIQUE, new List<TreasureSO>());
            for (int t = 0; t < staticMasterTreasureList.treasures.Length; t++)
            {
                treasuresByRarity[staticMasterTreasureList.treasures[t].rarity].Add(staticMasterTreasureList.treasures[t]);
            }
        }
        _treasureSpawned = false;
    }

    public static void ResetUniqueTreasures()
    {
        uniqueTreasuresSpawned.Clear();
    }

    public void SpawnTreasure()
    {
        if (_treasureSpawned)
        {
            return;
        }

        if (_masterTreasureList.chanceForNoItem > 0f)
        {
            if (Random.Range(0f,100f) <= _masterTreasureList.chanceForNoItem)
            {
                _treasureSpawned = true;
                return;
            }
        }

        if (_masterTreasureList.treasures.Length == 0)
        {
            throw new System.Exception("Master treasure list has 0 treasures in its treasure list!");
        }

        if (_manualSpawnTreasuresList != null)
        {
            SpawnTreasureFromManualSpawning();
            return;
        }

        List<TreasureSO.TreasureRarity> raritiesToSpawnFrom = new List<TreasureSO.TreasureRarity>();
        switch (_treasureRaritiesToChooseFrom.Length)
        {
            case 0:
                raritiesToSpawnFrom.Add(TreasureSO.TreasureRarity.COMMON);
                raritiesToSpawnFrom.Add(TreasureSO.TreasureRarity.UNCOMMON);
                raritiesToSpawnFrom.Add(TreasureSO.TreasureRarity.RARE);
                break;
            default:
                for (int r = 0; r < _treasureRaritiesToChooseFrom.Length; r++)
                {
                    if (_treasureRaritiesToChooseFrom[r] == TreasureSO.TreasureRarity.UNIQUE)
                    {
                        continue;
                    }
                    if (raritiesToSpawnFrom.Contains(_treasureRaritiesToChooseFrom[r]))
                    {
                        continue;
                    }
                    raritiesToSpawnFrom.Add(_treasureRaritiesToChooseFrom[r]);
                }
                break;
        }

        if (raritiesToSpawnFrom.Count == 0)
        {
            raritiesToSpawnFrom.Add(TreasureSO.TreasureRarity.COMMON);
            raritiesToSpawnFrom.Add(TreasureSO.TreasureRarity.UNCOMMON);
            raritiesToSpawnFrom.Add(TreasureSO.TreasureRarity.RARE);
        }

        if (_ableToSpawnUniqueTreasures)
        {
            raritiesToSpawnFrom.Add(TreasureSO.TreasureRarity.UNIQUE);
        }

        SpawnTreasureByRarity(raritiesToSpawnFrom.ToArray());
    }

    private void SpawnTreasureByRarity(params TreasureSO.TreasureRarity[] rarity)
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
                }
                validTreasures = RemoveAlreadySpawnedUniqueItems(treasures);
                break;

        }

        if (validTreasures.Length == 0)
        {
            Debug.LogError($"{gameObject.name} had no valid treasures to spawn!");
            _treasureRaritiesToChooseFrom = null;
            SpawnTreasure();
            return;
        }

        int treasurePicked = 0;
        if (_masterTreasureList.useWeightedSpawning)
        {
            treasurePicked = WeightedRandom(validTreasures);
        }
        else
        {
            treasurePicked = Random.Range(0, validTreasures.Length);
        }
        SpawnSpecificTreasure(validTreasures[treasurePicked]);
    }

    private void SpawnTreasureFromManualSpawning()
    {
        if (_treasureSpawned)
        {
            return;
        }
        if (_manualSpawnTreasuresList.treasures.Length == 0)
        {
            Debug.LogWarning($"Manual spawning list assigned for {gameObject.name} but no treasures are a part of the list!\nUsing set spawning settings!");
            _manualSpawnTreasuresList = null;
            SpawnTreasure();
            return;
        }
        if (_manualSpawnTreasuresList.treasures.Length == 1)
        {
            if (_manualSpawnTreasuresList.treasures[0].rarity == TreasureSO.TreasureRarity.UNIQUE)
            {
                Debug.LogWarning($"{gameObject.name} is using a list named {_manualSpawnTreasuresList.name} with only 1 treasure that is unique!\nPlease make sure the room spawning this is also unique!");
            }
            if (uniqueTreasuresSpawned.Contains(_manualSpawnTreasuresList.treasures[0]))
            {
                Debug.LogWarning($"{gameObject.name} is spawning a duplicate unique treasure!\nUsing default spawning options!");
                _manualSpawnTreasuresList = null;
                SpawnTreasure();
                return;
            }
            SpawnSpecificTreasure(_manualSpawnTreasuresList.treasures[0]);
            return;
        }

#if UNITY_EDITOR
        bool onlyUniqueTreasures = true;
        for (int t = 0; t < _manualSpawnTreasuresList.treasures.Length; t++)
        {
            if (_manualSpawnTreasuresList.treasures[t].rarity != TreasureSO.TreasureRarity.UNIQUE)
            {
                onlyUniqueTreasures = false;
                break;
            }
        }
        if (onlyUniqueTreasures)
        {
            Debug.LogWarning($"{gameObject.name} only has unique treasures in its manual treasure list {_manualSpawnTreasuresList.name}");
        }
#endif

        TreasureSO[] validTreasures = RemoveAlreadySpawnedUniqueItems(_manualSpawnTreasuresList.treasures);
        if (validTreasures.Length == 0)
        {
            Debug.LogWarning($"No valid treasures for manual spawning from list {_manualSpawnTreasuresList.name}\nUsing default spawning options!");
            _manualSpawnTreasuresList = null;
            SpawnTreasure();
            return;
        }

        int treasureChosen;
        if (_masterTreasureList.useWeightedSpawning)
        {
            treasureChosen = WeightedRandom(validTreasures);
        }
        else
        {
            treasureChosen = Random.Range(0, validTreasures.Length);
        }

        SpawnSpecificTreasure(validTreasures[treasureChosen]);
    }

    private void SpawnSpecificTreasure(TreasureSO treasureToSpawn)
    {
        if (_treasureSpawned)
        {
            return;
        }
        _treasureSpawned = true;
        if (treasureToSpawn.rarity == TreasureSO.TreasureRarity.UNIQUE)
        {
            uniqueTreasuresSpawned.Add(treasureToSpawn);
        }
        GameObject treasureGO = Instantiate(treasureToSpawn.worldObject, transform.position + Vector3.up * 0.5f, Quaternion.identity);
        treasureGO.transform.parent = FindObjectOfType<WorldItemsTag>().transform;
        treasureGO.GetComponent<WorldItem>().SpawnItem(transform.position + Vector3.up * 0.5f, treasureToSpawn);
    }

    private TreasureSO[] RemoveAlreadySpawnedUniqueItems(params TreasureSO[] treasures)
    {
        List<TreasureSO> validTreasures = new List<TreasureSO>();
        for (int t = 0; t < treasures.Length; t++)
        {
            if (treasures[t].rarity == TreasureSO.TreasureRarity.UNIQUE)
            {
                if (uniqueTreasuresSpawned.Contains(treasures[t]))
                {
                    continue;
                }
            }
            validTreasures.Add(treasures[t]);
        }

        return validTreasures.ToArray();
    }

    private TreasureSO[] RemoveAlreadySpawnedUniqueItems(List<TreasureSO> treasures)
    {
        return RemoveAlreadySpawnedUniqueItems(treasures.ToArray());
    }

    private int WeightedRandom(params TreasureSO[] treasures)
    {
        if (treasures.Length == 0)
        {
            throw new System.Exception($"No treasures in weighted random for {gameObject.name}!");
        }

        if (treasures.Length == 1)
        {
            return 0;
        }

        int chanceTotal = 0;
        for (int t = 0; t < treasures.Length; t++)
        {
            chanceTotal += treasures[t].spawnChancePoints;
        }
        int chancePicked = Random.Range(0, chanceTotal);
        for (int t = treasures.Length - 1; t >= 0; t--)
        {
            chancePicked -= treasures[t].spawnChancePoints;
            if (chancePicked <= 0)
            {
                return t;
            }
        }

        return Random.Range(0, treasures.Length);
    }

    private int WeightedRandom(List<TreasureSO> treasures)
    {
        return WeightedRandom(treasures.ToArray());
    }
}
