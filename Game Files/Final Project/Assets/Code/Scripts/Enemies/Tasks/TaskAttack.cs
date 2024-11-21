using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using BehaviorTree;

public class TaskAttack : Node
{
    private Enemy _enemyScript;
    private Transform _transform;
    private Animator _animator;

    private bool _isAttacking;

    public TaskAttack(Enemy enemyScript)
    {
        _enemyScript = enemyScript;
        _transform = _enemyScript.transform;
        _animator = enemyScript.animator;
        _enemyScript.SetIsAttacking(true);
    }

    public override NodeState Evaluate()
    {
        object t = GetData("target");
        if (t == null)
        {
            state = NodeState.FAILURE;
            return state;
        }

        if (_isAttacking)
        {
            
        }

        Transform target = (Transform)t;
        if (Vector3.Distance(_transform.position, target.position) <= 1f)
        {
            GameManager.Instance.GetManagedComponent<SuitSystem>().TakeDamage(50);
            Debug.Log("Take Damage");

            _enemyScript.SetIsStunned(true);
        }

        state = NodeState.RUNNING;
        _enemyScript.ChangeEnemyState(Enemy.EnemyState.Attacking);
        return state;
    }

}