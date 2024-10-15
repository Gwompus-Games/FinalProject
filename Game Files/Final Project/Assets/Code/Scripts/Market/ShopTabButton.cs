using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ShopTabButton : MonoBehaviour
{
    [field: SerializeField] public ShopUIManager.ShopTabEnum tab { get; private set; }
    private TMP_Text _buttonText;

    private void Awake()
    {
        _buttonText = GetComponentInChildren<TMP_Text>();
    }

    public void SetTab(ShopUIManager.ShopTabEnum tabToSet)
    {
        tab = tabToSet;
        string text = tab.ToString();
        _buttonText.text = text;
    }

    public void ButtonPressed()
    {
        GameManager.Instance.GetManagedComponent<ShopUIManager>().SwapToTab(tab);
    }
}
