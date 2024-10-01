using System.Collections.Generic;
using BehaviorTree;

public class AnglerFishBT : Tree
{
    private AnglerFish _anglerFish;

    private void Awake()
    {
        _anglerFish = GetComponent<AnglerFish>();
    }

    protected override Node SetupTree()
    {
        Node root = new Selector(new List<Node>
        {
            new Sequence(new List<Node>
            {
                new CheckPlayerInAttackRange(_anglerFish),
                new TaskAttack(_anglerFish),
            }),
            new Sequence(new List<Node>
            {
                new CheckPlayerInFOVRange(_anglerFish),
                new TaskGoToTarget(_anglerFish),
            }),
            new TaskPatrol(_anglerFish),
        });

        return root;
    }
}