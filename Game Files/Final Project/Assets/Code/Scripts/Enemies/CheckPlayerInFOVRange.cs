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
        object t = GetData("target");
        if (t == null)
        {
            Collider[] colliders = Physics.OverlapSphere(
                _transform.position, _enemyScript.fovRadius, _playerLayerMask);

            if (colliders.Length > 0)
            {
                parent.parent.SetData("target", colliders[0].transform);
                _animator.SetBool("Walking", true);
                state = NodeState.SUCCESS;
                return state;
            }

            state = NodeState.FAILURE;
            return state;
        }

        state = NodeState.SUCCESS;
        return state;
    }

}