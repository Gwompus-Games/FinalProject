using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using BehaviorTree;

public class TaskGoToTarget : Node
{
    private Transform _transform;
    private Enemy _enemyScript;

    public TaskGoToTarget(Enemy enemyScript)
    {
        _enemyScript = enemyScript;
        _transform = _enemyScript.transform;
    }

    public override NodeState Evaluate()
    {
        Transform target = (Transform)GetData("target");

        if (Vector3.Distance(_transform.position, target.position) > 0.5f)
        {
            _enemyScript.MoveToPoint(target.position);
        }

        state = NodeState.RUNNING;
        return state;
    }

}