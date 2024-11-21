using FMOD.Studio;
using System.Collections;
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

    //public float minAudioTime = 30f, maxAudioTime = 60f;
    //public float minSpotTime = 5f, currentSpotTime = 0f;

    //private float audioPlaytime, currentAudioTime;
    //private bool playSpotSFX = true;

    [Header("Assign")]
    public Animator animator;

    [Header("Debuging")]
    [SerializeField] private bool _debugMode;

    private EventInstance AFDistantInside;
    private EventInstance AFDistantOutside;
    private EventInstance AFSpotted;
    private EventInstance AFAttacking;
    private bool canPlayAmbiance = true;
    private float ambiancePlayTime = 60f;

    public enum EnemyState
    {
        Patrolling,
        Searching,
        Spotted,
        Attacking
    }
    public EnemyState currentEnemyState;

    protected virtual void Awake()
    {
        agent = GetComponent<NavMeshAgent>();

        UpdateNavMeshAgentSettings();
    }

    protected virtual void Start()
    {
        AFDistantInside = AudioManager.Instance.CreateEventInstance(FMODEvents.Instance.AFDistantInside);
        AFDistantOutside = AudioManager.Instance.CreateEventInstance(FMODEvents.Instance.AFDistantOutside);
        AFSpotted = AudioManager.Instance.CreateEventInstance(FMODEvents.Instance.AFSpotted);
        AFAttacking = AudioManager.Instance.CreateEventInstance(FMODEvents.Instance.AFAttacking);
    }

    public void ChangeEnemyState(EnemyState state)
    {
        //if (currentEnemyState == state)
        //    return;
        currentEnemyState = state;
        PLAYBACK_STATE ps;

        switch (state)
        {
            case EnemyState.Attacking:
                AFAttacking.getPlaybackState(out ps);
                if(ps.Equals(PLAYBACK_STATE.STOPPED))
                {
                    AFAttacking.start();
                    return;
                }
                break;
            case EnemyState.Spotted:
                AFSpotted.getPlaybackState(out ps);
                if (ps.Equals(PLAYBACK_STATE.STOPPED))
                {
                    AFSpotted.start();
                    return;
                }
                break;
            case EnemyState.Patrolling:
                if (!canPlayAmbiance)
                    return;
                StartCoroutine(PlayAmbiance());
                return; ;
            default:
                if (!canPlayAmbiance)
                    return;
                StartCoroutine(PlayAmbiance());
                break;
        }
    }

    private IEnumerator PlayAmbiance()
    {
        if(GameManager.Instance.isPlayerInsideFacility)
        {
            PLAYBACK_STATE ps;
            AFDistantInside.getPlaybackState(out ps);
            if (ps.Equals(PLAYBACK_STATE.STOPPED))
            {
                AFDistantInside.start();
            }
        }
        else
        {
            PLAYBACK_STATE ps;
            AFDistantOutside.getPlaybackState(out ps);
            if (ps.Equals(PLAYBACK_STATE.STOPPED))
            {
                AFDistantOutside.start();
            }
        }
        canPlayAmbiance = false;
        yield return new WaitForSeconds(ambiancePlayTime);
        canPlayAmbiance = true;
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
            if (_debugMode)
            {
                Debug.LogWarning("Invalid path");
            }
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

    public void AddHeartbeat()
    {
        if (_playerController == null)
        {
            return;
        }
        _playerController.AddHeartbeat(this);
    }

    public void RemoveHeartbeat()
    {
        if (_playerController == null)
        {
            return;
        }
        _playerController.RemoveHeartbeat(this);
    }
}
