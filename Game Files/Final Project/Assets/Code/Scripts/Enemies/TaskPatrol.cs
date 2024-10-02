using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using BehaviorTree;

public class TaskPatrol : Node
{
    private static int _roomLayerMask = 1 << 7;

    private Enemy _enemyScript;
    private Transform _transform;
    private Animator _animator;

    private int _lastPoint;

    private float _waitTime = 1f; // in seconds
    private float _waitCounter = 0f;
    private bool _waiting = false;

    public TaskPatrol(Enemy enemyScript)
    {
        _enemyScript = enemyScript;
        _transform = _enemyScript.transform;
        _animator = enemyScript.animator;
    }

    public override NodeState Evaluate()
    {
        if (_waiting)
        {
            _waitCounter += Time.deltaTime;
            if (_waitCounter >= _waitTime)
            {
                _waiting = false;
                _animator.SetBool("Walking", true);
            }
        }
        else
        {
            //Collider[] colliders = Physics.OverlapSphere(
            //    _transform.position, _enemyScript.patrolRadius, _roomLayerMask);

            //if (colliders.Length > 0)
            //{
            //    int randomIndex = Random.Range(0, colliders.Length);

            //    parent.parent.SetData("target", colliders[0].transform);
            //    _animator.SetBool("Walking", true);
            //    state = NodeState.SUCCESS;
            //    return state;
            //}

            //Transform wp = _waypoints[_currentWaypointIndex];
            //if (Vector3.Distance(_transform.position, wp.position) < 0.01f)
            //{
            //    _transform.position = wp.position;
            //    _waitCounter = 0f;
            //    _waiting = true;

            //    _currentWaypointIndex = (_currentWaypointIndex + 1) % _waypoints.Length;
            //    _animator.SetBool("Walking", false);
            //}
            //else
            //{
            //    _transform.position = Vector3.MoveTowards(_transform.position, wp.position, AnglerFishBT.speed * Time.deltaTime);
            //    _transform.LookAt(wp.position);
            //}
        }


        state = NodeState.RUNNING;
        return state;
    }

}