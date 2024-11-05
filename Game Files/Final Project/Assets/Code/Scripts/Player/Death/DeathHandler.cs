using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeathHandler : ManagedByGameManager
{
    [SerializeField] private List<GameObject> _deaths = new List<GameObject>();

    public override void Init()
    {
        if (_initilized)
        {
            return;
        }
        base.Init();

        HashSet<ParentDeath.DeathType> deaths = new HashSet<ParentDeath.DeathType>();
        for (int d = 0; d < _deaths.Count; d++)
        {
            if (!_deaths[d].TryGetComponent<ParentDeath>(out ParentDeath death))
            {
                throw new System.Exception("Non death assigned in the deaths list!");
            }

            if (deaths.Contains(death.causeOfDeath))
            {
                throw new System.Exception("Duplicate cause of deaths found in deaths list!");
            }

            deaths.Add(death.causeOfDeath);
        }

        ParentDeath.DeathType[] allTypes = Enum.GetValues(typeof(ParentDeath.DeathType)) as ParentDeath.DeathType[];
        for (int t = 0; t < allTypes.Length; t++)
        {
            if (_deaths.Find(x => x.GetComponent<ParentDeath>().causeOfDeath == allTypes[t]) == null)
            {
                //throw new Exception($"{allTypes[t]} type not found in deaths list!");
            }
        }

    }

    public void CreateDeathAnimation(ParentDeath.DeathType deathType)
    {
        GameObject death = _deaths.Find(x => x.GetComponent<ParentDeath>().causeOfDeath == deathType);
        if (death != null)
        {
            death = Instantiate(death, transform);
        }
    }

    public void DeathFinished(ParentDeath death)
    {
        if (death == null)
        {
            Debug.LogError("Death finished called with a null death! ");
            return;
        }

        if (!death.deathFinished)
        {
            Debug.LogError("Death finished called before death was finished!");
            return;
        }

        
    }
}
