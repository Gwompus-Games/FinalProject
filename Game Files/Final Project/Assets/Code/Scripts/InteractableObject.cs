using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InteractableObject : MonoBehaviour, IInteractable
{
    [SerializeField] private Canvas _popupCanvas;
    [SerializeField] private Image _popupImage;
    [SerializeField] private Sprite _popupSprite;
    [SerializeField] private float _popupDistance = 1.5f;

    protected virtual void Awake()
    {
        if (_popupCanvas == null)
        {
            _popupCanvas = GetComponentInChildren<Canvas>();
        }

        if (_popupImage == null) 
        { 
            _popupImage = GetComponentInChildren<Image>();
        }

        if (_popupCanvas == null)
        {
            throw new System.Exception("Canvas not found!");
        }
        if (_popupImage == null)
        {
            throw new System.Exception("Image not found!");
        }
        if (_popupSprite == null)
        {
            throw new System.Exception("Popup sprite not assigned");
        }

        _popupImage.sprite = _popupSprite;

        transform.tag = "Interactable";
    }

    protected virtual void Start()
    {
        DisablePopup();
    }

    public void EnablePopup()
    {
        PlayerController.PlayerPosUpdated += CalculatePositionAndDirectionOfPopup;
        _popupImage.enabled = true;
    }

    public void DisablePopup()
    {
        OnDisable();
        _popupImage.enabled = false;
    }

    public void CalculatePositionAndDirectionOfPopup(Vector3 playerPosition)
    {
        Vector3 direction = (playerPosition - transform.position).normalized;
        float distance = _popupDistance;
        float distanceBetween = Vector3.Distance(playerPosition, transform.position);
        float divisorDistance = 2.5f;
        if (distanceBetween - (distanceBetween / divisorDistance) < distance)
        {
            distance = distanceBetween - (distanceBetween / divisorDistance);
        }
        _popupCanvas.transform.localPosition = direction * distance;
        _popupCanvas.transform.forward = Camera.main.transform.forward;
        //_popupCanvas.transform.eulerAngles.
    }

    private void OnDisable()
    {
        PlayerController.PlayerPosUpdated -= CalculatePositionAndDirectionOfPopup;
    }
    private void OnDestroy()
    {
        OnDisable();
    }

    public virtual void Interact()
    {

    }
}
