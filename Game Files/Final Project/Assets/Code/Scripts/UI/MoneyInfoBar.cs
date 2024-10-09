using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoneyInfoBar : InfoBarTextElement
{
    private void OnEnable()
    {
        PlayerController.UpdateMoney += UpdateText;
    }

    private void OnDisable()
    {
        PlayerController.UpdateMoney -= UpdateText;
    }

    protected override void Start()
    {
        base.Start();
        UpdateText(PlayerController.Instance.money);
    }

    public override void UpdateText(int money)
    {
        base.UpdateText(money);
        string newText = _uiText.text;
        int insertPos = newText.IndexOf($": ");
        if (insertPos > 0) 
        {
            insertPos += 2;
            newText = newText.Insert(insertPos, "$");
        }
        _uiText.text = newText;
    }
}
