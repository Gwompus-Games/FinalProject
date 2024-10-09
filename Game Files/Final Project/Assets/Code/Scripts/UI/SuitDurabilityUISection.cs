using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SuitDurabilityUISection : MonoBehaviour
{
    private Image _sectionImage;

    private void Awake()
    {
        _sectionImage = GetComponent<Image>();
    }
}
