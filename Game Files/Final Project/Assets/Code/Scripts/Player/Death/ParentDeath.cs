using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class ParentDeath : ManagedObject
{
    public enum DeathType
    {
        Won,
        Suffocation,
        Beaten
        //Eaten
    }

    [field :SerializeField] public DeathType causeOfDeath { get; protected set; }
    [SerializeField] private GameObject _deathEffectsPrefab;
    [SerializeField] private float _deathTimeInSeconds = 5f;
    private float _currentTime;
    private Canvas _canvas;
    private GameObject _deathEffectGameObject;
    private DeathVisualComponent[] _deathVisualComponents;
    private DeathAudioComponent[] _deathAudioComponents;
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
        _canvas = FindFirstObjectByType<InventoryCanvasTag>().GetComponent<Canvas>();
    }

    public override void CustomStart()
    {
        base.CustomStart();
        _deathEffectGameObject = Instantiate(_deathEffectsPrefab, _canvas.transform);
        _deathVisualComponents = _deathEffectGameObject.GetComponentsInChildren<DeathVisualComponent>();
        _deathAudioComponents = _deathEffectGameObject.GetComponentsInChildren<DeathAudioComponent>();
        deathFinished = false;
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

        float alpha = _currentTime / _deathTimeInSeconds;
        if (_deathVisualComponents.Length > 0)
        {
            for (int dvc = 0; dvc < _deathVisualComponents.Length; dvc++)
            {
                _deathVisualComponents[dvc].SetAlpha(alpha);
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
        Destroy(_deathEffectGameObject);
        Destroy(gameObject);
    }
}
