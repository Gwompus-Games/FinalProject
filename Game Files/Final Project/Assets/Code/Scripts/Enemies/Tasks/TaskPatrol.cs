using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using BehaviorTree;
using static UnityEngine.GraphicsBuffer;

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

    private Transform currentRoom;

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
            if (currentRoom == null)
                FindNewRoom();

            if (Vector3.Distance(_transform.position, currentRoom.position) < 1f)
            {
                _waitCounter = 0f;
                _waiting = true;

                FindNewRoom();
                _animator.SetBool("Walking", false);
            }
            else
            {
                _enemyScript.MoveToPoint(currentRoom.position);
            }
        }

        state = NodeState.RUNNING;
        _enemyScript.ChangeEnemyState(Enemy.EnemyState.Patrolling);
        return state;
    }

    private void FindNewRoom()
    {
        Collider[] colliders = Physics.OverlapSphere(
                _transform.position, _enemyScript.patrolRadius, _roomLayerMask);

        if (colliders.Length > 0)
        {
            int randomIndex = Random.Range(0, colliders.Length);

            if (_enemyScript.HasValidPath(colliders[randomIndex].transform.position))
                currentRoom = colliders[randomIndex].transform;
            else
                FindNewRoom();
        }
    }

}