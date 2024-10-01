using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class Enemy : MonoBehaviour
{
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

    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();

        UpdateNavMeshAgentSettings();
    }

    public void MoveToPoint(Vector3 pos)
    {
        agent.destination = pos;
    }

    private void UpdateNavMeshAgentSettings()
    {
        agent.speed = moveSpeed;
        agent.acceleration = acceleration;
        agent.angularSpeed = turnSpeed;
    }
}
