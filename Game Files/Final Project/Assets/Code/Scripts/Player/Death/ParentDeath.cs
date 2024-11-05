using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class ParentDeath : MonoBehaviour
{
    public enum DeathType
    {
        Won,
        Suffocation,
        Beaten
        //Eaten
    }

    [field :SerializeField] public DeathType causeOfDeath { get; protected set; }
    public bool deathFinished { get; protected set; } = false;

    private void Update()
    {
        
    }

}
