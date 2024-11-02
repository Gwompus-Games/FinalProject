using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShopUIManager : ManagedByGameManager
{
    [SerializeField] private GameObject _shopTabsButtonSection;
    [SerializeField] private GameObject _buttonPrefab;
    [field: SerializeField] public Color ableToBuyBackgroundColour { get; private set; }
    [field: SerializeField] public Color ableToBuyTextColour { get; private set; }
    [field: SerializeField] public Color unableToBuyBackgroundColour { get; private set; }
    [field: SerializeField] public Color unableToBuyTextColour { get; private set; }
    [field: SerializeField] public Color cannotBuyBackgroundColour { get; private set; }
    [field: SerializeField] public Color cannotBuyTextColour { get; private set; }

    public enum ShopTabEnum
    {
        SHOP,
        SCRAPING,
        //AUCTIONING,
        REPAIRS
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


    public override void Init()
    {
        base.Init();
        _shopTabs = GetComponentsInChildren<ShopTab>();
    }

    public override void CustomStart()
    {
        base.CustomStart();
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
