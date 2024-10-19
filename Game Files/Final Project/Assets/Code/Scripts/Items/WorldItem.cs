using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(MeshRenderer))]
[RequireComponent(typeof(MeshFilter))]
[RequireComponent(typeof(Collider))]
[RequireComponent(typeof(Rigidbody))]
public class WorldItem : MonoBehaviour, IInteractable
{
    [SerializeField] private ItemDataSO _itemData;
    private MeshRenderer _meshRenderer;
    private Rigidbody _rigidbody;
    private Canvas _popupCanvas;
    private Image _popupImage;
    [SerializeField] private Sprite _popupSprite;
    [SerializeField] private float _popupDistance = 1.5f;
    public bool hidden { get; private set; } = false;

    protected void Awake()
    {
        _meshRenderer = GetComponent<MeshRenderer>();
        _rigidbody = GetComponent<Rigidbody>();
        _popupCanvas = GetComponentInChildren<Canvas>();
        _popupImage = GetComponentInChildren<Image>();

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

        if (_itemData != null)
        {
            AssignData(_itemData);
        }
        transform.tag = "Interactable";
    }

    protected virtual void Start()
    {
        DisablePopup();
    }

    public virtual void SpawnItem(Vector3 position, ItemDataSO itemData = null)
    {
        if (itemData != null)
        {
            AssignData(itemData);
        }
        transform.position = position;
        HideItem(false);
    }

    public void DespawnItem()
    {
        Destroy(gameObject);
    }

    protected void AssignData(ItemDataSO itemData)
    {
        _itemData = itemData;
        _rigidbody.mass = _itemData.density;
        _rigidbody.drag = _itemData.drag;
        _rigidbody.angularDrag = _itemData.angularDrag;
    }

    protected void HideItem(bool hideItem)
    {
        _meshRenderer.enabled = !hideItem;
    }

    public virtual void Interact()
    {
        GameManager.Instance.GetManagedComponent<InventoryController>().AddItemToInventory(_itemData);
        DespawnItem();
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
}
