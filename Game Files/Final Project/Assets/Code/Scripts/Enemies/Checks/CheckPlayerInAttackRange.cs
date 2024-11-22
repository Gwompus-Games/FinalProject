using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using BehaviorTree;

public class CheckPlayerInAttackRange : Node
{
    private Enemy _enemyScript;
    private Transform _transform;
    private Animator _animator;

    private float currentTime;

    public CheckPlayerInAttackRange(Enemy enemyScript)
    {
        _enemyScript = enemyScript;
        _transform = _enemyScript.transform;
        _animator = _enemyScript.animator;
        currentTime = 0;
    }

    public override NodeState Evaluate()
    {
        object t = GetData("target");
        if (t == null || _enemyScript.IsStunned() || _enemyScript.IsAttackOnCooldown())
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

        RaycastHit hit;
            
        if (Physics.Raycast(_transform.position + Vector3.up * 1.5f, target.position + Vector3.up * 1.5f - (_transform.position + Vector3.up * 1), out hit, _enemyScript.attackRadius, _enemyScript.raycastMask))
        {
            if (hit.transform == target.transform)
            {
                _enemyScript.SetAttackTargetPos(target.position);
                _enemyScript.SetIsAttacking(true);
                _enemyScript.StopMoving();
                _enemyScript.SetAttackStartPos(_transform.position);
                _animator.SetTrigger("Attacking");
                _animator.SetBool("Walking", false);

                state = NodeState.SUCCESS;
                _enemyScript.ChangeEnemyState(Enemy.EnemyState.Attacking);
                return state;
            }
        }

        state = NodeState.FAILURE;
        return state;
    }
}