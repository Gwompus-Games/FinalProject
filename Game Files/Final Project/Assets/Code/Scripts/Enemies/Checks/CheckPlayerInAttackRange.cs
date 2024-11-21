using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using BehaviorTree;

public class CheckPlayerInAttackRange : Node
{
    private Enemy _enemyScript;
    private Transform _transform;
    private Animator _animator;

    public CheckPlayerInAttackRange(Enemy enemyScript)
    {
        _enemyScript = enemyScript;
        _transform = _enemyScript.transform;
        _animator = _enemyScript.animator;
    }

    public override NodeState Evaluate()
    {
        object t = GetData("target");
        if (t == null)
        {
            state = NodeState.FAILURE;
            return state;
        }

        if (_enemyScript.IsAttacking())
        {
            state = NodeState.SUCCESS;
            _enemyScript.ChangeEnemyState(Enemy.EnemyState.Spotted);
            return state;
        }

        Transform target = (Transform)t;
        if (Vector3.Distance(_transform.position, target.position) <= _enemyScript.attackRadius)
        {
            _enemyScript.SetAttackTargetPos(target.position);
            _enemyScript.SetIsAttacking(true);
            _animator.SetTrigger("Attacking");
            _animator.SetBool("Walking", false);

            state = NodeState.SUCCESS;
            _enemyScript.ChangeEnemyState(Enemy.EnemyState.Spotted);
            return state;
        }

        state = NodeState.FAILURE;
        return state;
    }

}