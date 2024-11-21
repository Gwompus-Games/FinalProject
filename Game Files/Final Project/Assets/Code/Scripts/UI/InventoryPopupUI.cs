using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(CanvasGroup))]
public class InventoryPopupUI : MonoBehaviour
{
    private CanvasGroup _canvasGroup;
    private RectTransform _rectTransform;
    public float popupHoverTimeInSeconds { get; private set; } = 1f;
    private Vector2 _popupPositionOffset;
    private LayoutElement _spriteLayoutElement;
    private float _spriteScale = 3f;

    [Serializable]
    public struct PopupInfo
    {
        public CanvasGroup canvasGroup;
        public TMP_Text titleTextUI;
        public TMP_Text itemDescription;
        public Image itemImage;
    }

    [Serializable]
    public struct OxygenTankPopupInfo
    {
        public PopupInfo popupInfo;
        public Image oxygenFillBar;
        public TMP_Text fillPercentUI;
    }

    [SerializeField] private PopupInfo _defaultItemPopup;
    [SerializeField] private OxygenTankPopupInfo _oxygenTankPopup;

    private void Awake()
    {
        _canvasGroup = GetComponent<CanvasGroup>();
        _rectTransform = GetComponent<RectTransform>();
        _popupPositionOffset = new Vector2(-_rectTransform.sizeDelta.x * 2f / 3f, 0f);
    }

    private void Start()
    {
        DisablePopup();
    }

    public void EnablePopup(Vector2 position)
    {
        _canvasGroup.alpha = 1f;
        UpdatePopup(position);
    }

    public void DisablePopup()
    {
        _canvasGroup.alpha = 0f;
    }

    public void SetPopupData(InventoryItem item)
    {
        II_OxygenTank oxygenTankItem = item as II_OxygenTank;
        if (oxygenTankItem != null)
        {
            DisablePopupSection(_defaultItemPopup);
            PopulatePopup(_oxygenTankPopup, oxygenTankItem);
        }
        else
        {
            DisablePopupSection(_oxygenTankPopup.popupInfo);
            PopulatePopup(_defaultItemPopup, item);
        }
    }

    private void DisablePopupSection(PopupInfo popup)
    {
        popup.canvasGroup.alpha = 0f;
    }

    private void PopulatePopup(PopupInfo popup, InventoryItem item)
    {
        popup.canvasGroup.alpha = 1f;
        popup.titleTextUI.text = item.itemData.itemName;
        if (item.itemData.inventoryItemSprite != null)
        {
            popup.itemImage.sprite = item.itemData.inventoryItemSprite;
        }
        else
        {
            popup.itemImage.sprite = item.GetItemSprite();
        }
        popup.itemImage.color = item.GetItemColour();
        popup.itemDescription.text = item.itemData.itemDescription;
    }
    
    private void PopulatePopup(OxygenTankPopupInfo oxygenPopup, II_OxygenTank oxygenItem)
    {
        PopulatePopup(oxygenPopup.popupInfo, oxygenItem);
        oxygenPopup.fillPercentUI.text = $"Fill Percent: {oxygenItem.oxygenLeftPercent.Split(".")[0]}%";
        oxygenPopup.oxygenFillBar.fillAmount = oxygenItem.oxygenFillAmount;
    }

    private void ResizeSprite(PopupInfo popup)
    {
        if (_spriteLayoutElement == null)
        {
            if (!popup.itemImage.TryGetComponent<LayoutElement>(out _spriteLayoutElement))
            {
                throw new Exception("Error with getting sprite's layout element.");
            }
        }

        float x = popup.itemImage.sprite.rect.x;
        float y = popup.itemImage.sprite.rect.y;

        _spriteLayoutElement.preferredWidth = x;
        _spriteLayoutElement.preferredHeight = y;
    }

    public void UpdatePopup(Vector2 position)
    {
        _rectTransform.position = position + _popupPositionOffset;
    }

    public void UpdatePopup(Vector2 position, II_OxygenTank oxygenItem)
    {
        UpdatePopup(position);
        _oxygenTankPopup.fillPercentUI.text = $"Fill Percent: {oxygenItem.oxygenLeftPercent.Split(".")[0]}%";
        _oxygenTankPopup.oxygenFillBar.fillAmount = oxygenItem.oxygenFillAmount;
    }
}
