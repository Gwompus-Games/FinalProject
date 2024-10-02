using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnglerFish : Enemy
{
    public static AnglerFish Instance;

    protected override void Awake()
    {
        base.Awake();
    }

    public override void SetupEnemy()
    {
        Instance = this;

        base.SetupEnemy();
    }
}
