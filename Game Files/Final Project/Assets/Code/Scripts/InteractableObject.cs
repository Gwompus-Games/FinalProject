using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class InteractableObject : MonoBehaviour, IInteractable
{
    [SerializeField] private Canvas _popupCanvas;
    [SerializeField] private Image _popupImage;
    [SerializeField] private Sprite _popupSprite;
    [SerializeField] private float _popupDistance = 1.5f;
    [SerializeField] private TMP_Text _popupText;
    [SerializeField] private char _popupKey = 'E';
    private Transform _playerCamera;
    public bool popupEnabled { get; private set; } = true;

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

        if (_popupText == null && _popupImage != null)
        {
            _popupText = _popupImage.gameObject.GetComponentInChildren<TMP_Text>();
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
        if (_popupText == null)
        {
            throw new System.Exception("No text object found!");
        }

        _popupImage.sprite = _popupSprite;
        _popupText.text = _popupKey.ToString();

        transform.tag = "Interactable";
        _playerCamera = Camera.main.transform;
    }

    protected virtual void Start()
    {
        DisablePopup();
    }

    public virtual void EnablePopup()
    {
        if (popupEnabled)
        {
            return;
        }
        popupEnabled = true;
        PlayerController.PlayerPosUpdated += CalculatePositionAndDirectionOfPopup;
        CalculatePositionAndDirectionOfPopup(_playerCamera.position);
        _popupImage.gameObject.SetActive(true);
    }

    public void DisablePopup()
    {
        if (!popupEnabled) 
        { 
            return;
        }
        popupEnabled = false;
        OnDisable();
        _popupImage.gameObject.SetActive(false);
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
        _popupCanvas.transform.position = transform.position + (direction * distance);
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
