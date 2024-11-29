using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using BehaviorTree;

public class CheckPlayerInFOVRange : Node
{
    private static int _playerLayerMask = 1 << 3;

    private Transform _transform;
    private Enemy _enemyScript;
    private Animator _animator;

    public CheckPlayerInFOVRange(Enemy enemyScript)
    {
        _enemyScript = enemyScript;
        _transform = _enemyScript.transform;
        _animator = _enemyScript.animator;
    }

    public override NodeState Evaluate()
    {
        if (_enemyScript.IsStunned())
        {
            state = NodeState.FAILURE;
            return state;
        }

        Collider[] colliders = Physics.OverlapSphere(
                _transform.position, _enemyScript.fovRadius, _playerLayerMask);

        object t = GetData("target");

        if (colliders.Length > 0)
        {
            if (t == null) 
            {
                RaycastHit hit;

                if (Physics.Raycast(_transform.position + Vector3.up * 1.5f, (colliders[0].transform.position + Vector3.up * 1.5f) - (_transform.position + Vector3.up * 1.5f), out hit, _enemyScript.fovRadius, _enemyScript.raycastMask))
                {
                    Debug.Log(hit.transform.name);
                    if (hit.transform == colliders[0].transform)
                    {
                        _enemyScript.StartCoroutine(_enemyScript.AttackCooldown());
                        parent.parent.SetData("target", colliders[0].transform);
                        _animator.SetBool("Walking", true);
                        state = NodeState.SUCCESS;
                        return state;
                    }
                }
            }
        }
        else 
        {
            if (t != null) 
            {
                parent.parent.SetData("target", null);
            }
            state = NodeState.FAILURE;
            return state;
        }

        if (t != null) 
        {
            state = NodeState.SUCCESS;
            return state;
        }

        state = NodeState.FAILURE;
        return state;
    }

}