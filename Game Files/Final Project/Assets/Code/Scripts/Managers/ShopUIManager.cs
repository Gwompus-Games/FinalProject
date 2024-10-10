using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShopUIManager : MonoBehaviour
{
    [SerializeField] private GameObject _shopTabsButtonSection;
    [SerializeField] private GameObject _buttonPrefab;

    public enum ShopTabEnum
    {
        SHOP,
        SCRAPING,
        AUCTIONING
    }

    public ShopTabEnum currentTab
    {
        get
        {
            return _curTab;
        }
        private set
        {
            _curTab = value;
            SwapTabUI(_curTab);
        }
    }
    private ShopTabEnum _curTab;
    private ShopTab[] _shopTabs;


    private void Awake()
    {
        _shopTabs = GetComponentsInChildren<ShopTab>();
    }

    private void Start()
    {
        currentTab = ShopTabEnum.SCRAPING;
        SwapToTab(currentTab);

        ShopTabEnum[] shopTabs = Enum.GetValues(typeof(ShopTabEnum)) as ShopTabEnum[];
        for (int t = 0; t < shopTabs.Length; t++)
        {
            ShopTabButton button = Instantiate(_buttonPrefab, _shopTabsButtonSection.transform).GetComponent<ShopTabButton>();
            button.SetTab(shopTabs[t]);
        }
    }

    private void SwapTabUI(ShopTabEnum tabToSwitchTo)
    {
        for (int t = 0; t < _shopTabs.Length; t++)
        {
            if (_shopTabs[t].tab == tabToSwitchTo)
            {
                _shopTabs[t].gameObject.SetActive(true);
            }
            else
            {
                _shopTabs[t].gameObject.SetActive(false);
            }
        }
    }

    public void SwapToTab(ShopTabEnum tabToSwapTo)
    {
        currentTab = tabToSwapTo;
    }
}
