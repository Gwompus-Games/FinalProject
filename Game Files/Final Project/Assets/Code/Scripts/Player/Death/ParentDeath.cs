using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParentDeath : MonoBehaviour
{
    public enum DeathType
    {
        Suffocation,
        Beaten
        //Eaten
    }

    [field :SerializeField] public DeathType causeOfDeath { get; protected set; }
    [SerializeField] protected Animation _deathAnimation;

    private void Update()
    {
        
    }


    //[Serializable]
    //protected struct DeathStats
    //{
    //    public DeathType deathType;
    //    public float deathTimeInSeconds;
    //}


}
