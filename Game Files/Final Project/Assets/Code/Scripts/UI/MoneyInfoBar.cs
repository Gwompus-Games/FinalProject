using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class MoneyInfoBar : InfoBarTextElement
{
    protected override void OnEnable()
    {
        if (!_initilized)
        {
            return;
        }
        if (_enabled)
        {
            return;
        }
        base.OnEnable();
        PlayerController.UpdateMoney += UpdateText;
        UpdateText(GameManager.Instance.GetManagedComponent<PlayerController>().money);
    }

    protected override void OnDisable()
    {
        if (!_initilized)
        {
            return;
        }
        if (!_enabled)
        {
            return;
        }
        base.OnDisable();
        PlayerController.UpdateMoney -= UpdateText;
    }

    public override void Init()
    {
        base.Init();
    }

    public override void CustomStart()
    {
        base.CustomStart();
        OnEnable();
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
