using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using BehaviorTree;

public class TaskAttack : Node
{
    private Enemy _enemyScript;
    private Transform _transform;
    private Animator _animator;

    private float _attackTime = 2f;
    private float _attackCounter = 0f;

    public TaskAttack(Enemy enemyScript)
    {
        _enemyScript = enemyScript;
        _transform = _enemyScript.transform;
        _animator = enemyScript.animator;
    }

    public override NodeState Evaluate()
    {
        Transform target = (Transform)GetData("target");

        _attackCounter += Time.deltaTime;
        if (_attackCounter >= _attackTime)
        {
            PlayerController.Instance.RestartGame();
            //bool enemyIsDead = _playerController.TakeHit();
            //if (enemyIsDead)
            //{
            //    ClearData("target");
            //    _animator.SetBool("Attacking", false);
            //    _animator.SetBool("Walking", true);
            //}
            //else
            //{
            //    _attackCounter = 0f;
            //}
        }

        state = NodeState.RUNNING;
        return state;
    }

}