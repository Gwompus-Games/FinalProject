using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using BehaviorTree;

public class TaskIdle : Node
{
    private Enemy _enemyScript;

    private float _idleCounter = 0;

    public TaskIdle(Enemy enemyScript)
    {
        _enemyScript = enemyScript;
    }

    public override NodeState Evaluate()
    {
        Transform target = (Transform)GetData("target");

        _idleCounter += Time.deltaTime;
        if (_idleCounter >= _enemyScript.attackStunTime)
        {
            _enemyScript.SetIsStunned(false);
            _idleCounter = 0;
        }

        state = NodeState.RUNNING;
        _enemyScript.ChangeEnemyState(Enemy.EnemyState.Idle);
        return state;
    }

}