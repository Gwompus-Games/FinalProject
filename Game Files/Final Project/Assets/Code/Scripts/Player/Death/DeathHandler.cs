using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeathHandler : ManagedByGameManager
{
    [SerializeField] private List<GameObject> _deaths = new List<GameObject>();
    public bool finished { get; private set; }
    private GameObject _deathObject;

    public override void Init()
    {
        if (_initilized)
        {
            return;
        }
        base.Init();

        HashSet<DeathObject.DeathType> deaths = new HashSet<DeathObject.DeathType>();
        for (int d = 0; d < _deaths.Count; d++)
        {
            if (!_deaths[d].TryGetComponent<DeathObject>(out DeathObject death))
            {
                throw new System.Exception("Non death assigned in the deaths list!");
            }

            if (deaths.Contains(death.causeOfDeath))
            {
                throw new System.Exception("Duplicate cause of deaths found in deaths list!");
            }

            deaths.Add(death.causeOfDeath);
        }

        DeathObject.DeathType[] allTypes = Enum.GetValues(typeof(DeathObject.DeathType)) as DeathObject.DeathType[];
        for (int t = 0; t < allTypes.Length; t++)
        {
            if (_deaths.Find(x => x.GetComponent<DeathObject>().causeOfDeath == allTypes[t]) == null)
            {
                throw new Exception($"{allTypes[t]} type not found in deaths list!");
            }
        }
    }

    public void CreateDeath(DeathObject.DeathType deathType)
    {
        if (_deathObject != null)
        {
            return;
        }
        finished = false;
        _deathObject = _deaths.Find(x => x.GetComponent<DeathObject>().causeOfDeath == deathType);
        if (_deathObject != null)
        {
            _deathObject = Instantiate(_deathObject, transform);
            DeathObject death = _deathObject.GetComponent<DeathObject>();
            death.SetDeathHandler(this);
            death.Init();
            death.CustomStart();
        }
    }

    public void DeathFinished(DeathObject death)
    {
        if (death == null)
        {
            Debug.LogError("Death finished called with a null death!");
            return;
        }

        if (!(death.deathFinished == true))
        {
            Debug.LogError("Death finished called before death was finished!");
            return;
        }

        finished = true;
    }
}
