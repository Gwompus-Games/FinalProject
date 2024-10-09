using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class fillableUISection : MonoBehaviour
{
    [SerializeField] private Image _fillImage;
    [SerializeField] private Image _emptyImage;

    private void Awake()
    {

    }

    public void SetFillAmount(float fillAmount)
    {
        fillAmount = Mathf.Clamp(fillAmount, 0, 1);
        _fillImage.fillAmount = fillAmount;
    }

    public void SetColours(Color fillColour, Color emptyColour)
    {
        _fillImage.color = fillColour;
        _emptyImage.color = emptyColour;
    }
}
