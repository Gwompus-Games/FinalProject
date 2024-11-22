using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using BehaviorTree;

public class CheckIsStunned : Node
{
    private Enemy _enemyScript;

    public CheckIsStunned(Enemy enemyScript)
    {
        _enemyScript = enemyScript;
    }

    public override NodeState Evaluate()
    {
        if (_enemyScript.IsStunned())
        {
            state = NodeState.SUCCESS;
            return state;
        }

        state = NodeState.FAILURE;
        return state;
    }

}