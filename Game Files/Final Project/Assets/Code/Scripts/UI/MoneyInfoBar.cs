using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoneyInfoBar : InfoBarTextElement
{
    protected override void OnEnable()
    {
        base.OnEnable();
        PlayerController.UpdateMoney += UpdateText;
        UpdateText(GameManager.PlayerControllerInstance.money);
    }

    protected override void OnDisable()
    {
        base.OnDisable();
        PlayerController.UpdateMoney -= UpdateText;
    }

    protected override void Start()
    {
        base.Start();
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
