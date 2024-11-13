using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class DeathObject : ManagedObject
{
    public enum DeathType
    {
        Won,
        Suffocation,
        Beaten
        //Eaten
    }

    [field :SerializeField] public DeathType causeOfDeath { get; protected set; }
    [SerializeField] private float _deathTimeInSeconds = 5f;
    [SerializeField] private GameObject[] _visualDeathEffects;
    private float _currentTime;
    private Canvas _canvas;
    private GameObject[] _visualDeathGameObjects;
    private DeathComponent[] _deathComponents;
    private DeathHandler _deathHandler;

    public bool? deathFinished { get; protected set; }

    public void SetDeathHandler(DeathHandler deathHandler)
    {
        _deathHandler = deathHandler;
        deathFinished = null;
    }

    public override void Init()
    {
        base.Init();
        _canvas = FindFirstObjectByType<MainCanvasTag>().GetComponent<Canvas>();
    }

    public override void CustomStart()
    {
        deathFinished = false;
        base.CustomStart();
        List<DeathComponent> deathComponents = new List<DeathComponent>(GetComponents<DeathComponent>());
        if (_visualDeathEffects.Length > 0 &&
            _canvas != null)
        {
            List<GameObject> visualGameObjects = new List<GameObject>();
            for (int d = 0; d < _visualDeathEffects.Length; d++)
            {
                GameObject currentObject = Instantiate(_visualDeathEffects[d], _canvas.transform);
                visualGameObjects.Add(currentObject);
                deathComponents.Add(currentObject.GetComponent<DeathComponent>());
            }
            _visualDeathGameObjects = visualGameObjects.ToArray();
        }
        _deathComponents = deathComponents.ToArray();

        for (int d = 0; d < _deathComponents.Length; d++)
        {
            _deathComponents[d].StartDeathComponent();
        }
    }

    protected virtual void Update()
    {
        if (!_initilized)
        {
            return;
        }

        if (deathFinished != false)
        {
            return;
        }

        float normalizedTime = _currentTime / _deathTimeInSeconds;
        if (_deathComponents.Length > 0)
        {
            for (int d = 0; d < _deathComponents.Length; d++)
            {
                _deathComponents[d].UpdateDeathComponent(normalizedTime);
            }
        }

        _currentTime += Time.deltaTime;
        if (_currentTime > _deathTimeInSeconds)
        {
            CleanUp();
        }
    }

    protected void CleanUp()
    {
        deathFinished = true;
        if (_deathComponents.Length > 0)
        {
            for (int d = 0; d < _deathComponents.Length; d++)
            {
                _deathComponents[d].EndDeathComponent();
            }
        }
        _deathHandler.DeathFinished(this);
        if (_visualDeathGameObjects.Length > 0)
        {
            for (int gO = 0; gO < _visualDeathGameObjects.Length; gO++)
            {
                Destroy(_visualDeathGameObjects[gO]);
            }
        }
        Destroy(gameObject);
    }
}
