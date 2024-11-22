using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using BehaviorTree;

public class TaskAttack : Node
{
    private Enemy _enemyScript;
    private Transform _transform;
    private Animator _animator;

    private float _currentTime;

    public TaskAttack(Enemy enemyScript)
    {
        _enemyScript = enemyScript;
        _transform = _enemyScript.transform;
        _animator = enemyScript.animator;
        _currentTime = 0;
    }

    public override NodeState Evaluate()
    {
        object t = GetData("target");
        if (t == null || _enemyScript.IsAttackOnCooldown())
        {
            state = NodeState.FAILURE;
            return state;
        }

        Transform target = (Transform)t;

        Debug.Log(TimeManagement());

        if (TimeManagement() >= 1) 
        {
            _enemyScript.StartCoroutine(_enemyScript.AttackCooldown());
            _enemyScript.SetIsStunned(true);
            _currentTime = 0;
            
            if (Vector3.Distance(target.position, _enemyScript.AttackTargetPos()) <= 0.5f)
            {
                GameManager.Instance.GetManagedComponent<SuitSystem>().TakeDamage(50);
                _enemyScript.SetIsAttacking(false);
                Debug.Log("Take Damage");
            }

            state = NodeState.FAILURE;
            return state;
        }
        else 
        {
            _currentTime += Time.deltaTime;
            MoveToTarget();
        }

        state = NodeState.RUNNING;
        _enemyScript.ChangeEnemyState(Enemy.EnemyState.Attacking);
        return state;
    }

    private void MoveToTarget() 
    {
        _transform.position = Vector3.MoveTowards(
            _enemyScript.AttackStartPos(), _enemyScript.AttackTargetPos(), 
            _enemyScript.attackCurve.Evaluate(TimeManagement())
            );
    }

    private float TimeManagement() 
    {
        return _currentTime / _enemyScript.attackDuration;
    }

}