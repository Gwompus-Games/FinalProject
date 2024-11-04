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
    public float minAudioTime = 30f, maxAudioTime = 60f;
    public float minSpotTime = 5f, currentSpotTime = 0f;

    private float audioPlaytime, currentAudioTime;
    private bool playSpotSFX = true;

    [Header("Assign")]
    public Animator animator;

    [Header("Debuging")]
    [SerializeField] private bool _debugMode;

    private EventInstance AFDistantInside;
    private EventInstance AFDistantOutside;
    private EventInstance AFSpotted;
    private EventInstance AFAttacking;

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
        ResetAudioTimes();

        AFDistantInside = GameManager.Instance.GetManagedComponent<AudioManager>().CreateEventInstance(GameManager.Instance.GetManagedComponent<FMODEvents>().AFDistantInside);
        AFDistantOutside = GameManager.Instance.GetManagedComponent<AudioManager>().CreateEventInstance(GameManager.Instance.GetManagedComponent<FMODEvents>().AFDistantOutside);
        AFSpotted = GameManager.Instance.GetManagedComponent<AudioManager>().CreateEventInstance(GameManager.Instance.GetManagedComponent<FMODEvents>().AFSpotted);
        AFAttacking = GameManager.Instance.GetManagedComponent<AudioManager>().CreateEventInstance(GameManager.Instance.GetManagedComponent<FMODEvents>().AFAttacking);
    }

    private void Update()
    {
        currentAudioTime += Time.deltaTime;
    }

    public void ChangeEnemyState(EnemyState state)
    {
        if (currentEnemyState == state)
            return;

        currentEnemyState = state;
        PLAYBACK_STATE PSInside, PSOutside, PSSpotted, PSAttacking;
        AFSpotted.getPlaybackState(out PSSpotted);
        AFAttacking.getPlaybackState(out PSAttacking);
        AFDistantInside.getPlaybackState(out PSInside);
        AFDistantOutside.getPlaybackState(out PSOutside);

        switch (state)
        {
            case EnemyState.Attacking:
                AFAttacking.set3DAttributes(FMODUnity.RuntimeUtils.To3DAttributes(gameObject));
                if (PSAttacking.Equals(PLAYBACK_STATE.STOPPED))
                {
                    if (PSSpotted.Equals(PLAYBACK_STATE.PLAYING))
                        AFSpotted.stop(STOP_MODE.ALLOWFADEOUT);
                    AFAttacking.start();
                }
                break;
            case EnemyState.Spotted:
                if(playSpotSFX)
                {
                    AFSpotted.set3DAttributes(FMODUnity.RuntimeUtils.To3DAttributes(gameObject));
                    if (PSSpotted.Equals(PLAYBACK_STATE.STOPPED))
                    {
                        if (PSAttacking.Equals(PLAYBACK_STATE.PLAYING))
                            return;
                        AFSpotted.start();
                    }
                }
                break;
            case EnemyState.Patrolling:
                if (currentAudioTime < audioPlaytime || PSAttacking.Equals(PLAYBACK_STATE.PLAYING) || PSSpotted.Equals(PLAYBACK_STATE.PLAYING))
                    return;
                AFDistantInside.set3DAttributes(FMODUnity.RuntimeUtils.To3DAttributes(gameObject));
                if(GameManager.Instance.isPlayerInsideFacility)
                {
                    if (PSInside.Equals(PLAYBACK_STATE.STOPPED))
                    {
                        if (PSOutside.Equals(PLAYBACK_STATE.PLAYING))
                            AFDistantOutside.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
                        AFDistantInside.start();
                        ResetAudioTimes();
                    }        
                }
                else
                {
                    if (PSOutside.Equals(PLAYBACK_STATE.STOPPED))
                    {
                        if (PSInside.Equals(PLAYBACK_STATE.PLAYING))
                            AFDistantInside.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
                        AFDistantOutside.start();
                        ResetAudioTimes();
                    }
                }
                break;
        }
    }

    private IEnumerator ResetSpotTime()
    {
        playSpotSFX = false;
        while(currentSpotTime < minSpotTime)
        {
            currentSpotTime += Time.deltaTime;
            yield return null;
        }
        playSpotSFX = true;
    }

    private void ResetAudioTimes()
    {
        audioPlaytime = Random.Range(minAudioTime, maxAudioTime);
        currentAudioTime = 0;
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
        if (_playerController.CheckHeartbeatInList(this))
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
        if (!_playerController.CheckHeartbeatInList(this))
        {
            return;
        }
        _playerController.RemoveHeartbeat(this);
    }
}
