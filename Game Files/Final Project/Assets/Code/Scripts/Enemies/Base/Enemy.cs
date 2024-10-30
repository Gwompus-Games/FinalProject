using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class Enemy : MonoBehaviour, IHeartbeat
{
    protected PlayerController _playerController;
    private NavMeshAgent agent;

    [Header("Settings")]
    public float moveSpeed = 3f;
    public float acceleration = 6;
    public float turnSpeed = 80;
    public float patrolRadius = 20f;
    public float fovRadius = 6f;
    public float attackRadius = 1f;

    [Header("Assign")]
    public Animator animator;

    [Header("Debuging")]
    [SerializeField] private bool _debugMode;

    protected virtual void Awake()
    {
        agent = GetComponent<NavMeshAgent>();

        UpdateNavMeshAgentSettings();
    }

    public virtual void SetupEnemy()
    {
        _playerController = GameManager.Instance.GetManagedComponent<PlayerController>();
    }

    public void MoveToPoint(Vector3 pos, float speedMultiplier = 1)
    {
        if (agent.isStopped)
            agent.isStopped = false;

        agent.destination = pos;
    }

    public void StopMoving()
    {
        agent.isStopped = true;
    }

    private void UpdateNavMeshAgentSettings()
    {
        agent.speed = moveSpeed;
        agent.acceleration = acceleration;
        agent.angularSpeed = turnSpeed;
    }

    public bool HasValidPath(Vector3 targetPos)
    {
        NavMeshPath navMeshPath = new NavMeshPath();
        agent.CalculatePath(targetPos, navMeshPath);
        
        if (navMeshPath.status != NavMeshPathStatus.PathComplete)
        {
            Debug.LogWarning("Invalid path");
            return false;
        }
        else
        {
            if (_debugMode)
            {
                Debug.Log("Valid path");
            }
            return true;
        }
    }
}
