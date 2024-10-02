using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnglerFish : Enemy
{
    public static AnglerFish instance;

    private void Awake()
    {
        instance = this;
    }
}
