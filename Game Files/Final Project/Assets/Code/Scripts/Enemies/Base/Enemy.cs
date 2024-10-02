using FMODUnity;
using System.Collections;
using System.Collections.Generic;
using TMPro;
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

    [Header("FMOD")]
    private StudioEventEmitter emitter;

    protected virtual void Awake()
    {
        agent = GetComponent<NavMeshAgent>();

        UpdateNavMeshAgentSettings();

        //FMOD
        emitter = AudioManager.instance.InitializeEventEmitter(FMODEvents.instance.heartbeat, gameObject);
    }

    private void Start()
    {
        if (!HasValidPath())
        {
            DungeonGenerator.Instance.EnemyFailedToSpawn();
            Destroy(gameObject);
        }
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

    public bool HasValidPath()
    {
        NavMeshPath navMeshPath = new NavMeshPath();
        agent.CalculatePath(PlayerController.INSTANCE.transform.position, navMeshPath);
        
        if (navMeshPath.status != NavMeshPathStatus.PathComplete)
        {
            print("Invalid path");
            return false;
        }
        else
        {
            print("Valid path");
            return true;
        }
    }

    #region FMOD
    private void OnTriggerEnter(Collider other)
    {
        if (emitter && other.CompareTag("Player"))
        {
            emitter.Play();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (emitter && other.CompareTag("Player"))
        {
            emitter.Stop();
        }
    }
    #endregion
}
