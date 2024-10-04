using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class SellValueUIScript : MonoBehaviour
{
    private TMP_Text _valueText;

    private void Awake()
    {
        _valueText = GetComponent<TMP_Text>();
    }

    public void UpdateUI(int value)
    {
        if (_valueText == null)
        {
            return;
        }

        _valueText.text = "$" + value.ToString();
    }
}
