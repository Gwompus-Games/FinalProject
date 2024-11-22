using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using BehaviorTree;

public class TaskIdle : Node
{
    private Enemy _enemyScript;
    private Animator _animator;

    private float _idleCounter = 0;

    public TaskIdle(Enemy enemyScript)
    {
        _enemyScript = enemyScript;
        _animator = enemyScript.animator;
    }

    public override NodeState Evaluate()
    {
        Transform target = (Transform)GetData("target");

        _idleCounter += Time.deltaTime;

        //_enemyScript.StopMoving();
        //Debug.Log("idling");
        if (_idleCounter >= _enemyScript.attackStunTime)
        {
            _enemyScript.SetIsStunned(false);
            _animator.SetBool("Walking", true);
            _idleCounter = 0;
        }

        state = NodeState.RUNNING;
        _enemyScript.ChangeEnemyState(Enemy.EnemyState.Idle);
        return state;
    }

}