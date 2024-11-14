using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class VolumeSlider : MonoBehaviour
{
    private Slider slider;
    public AudioManager.VolumeBus volumeBus;
    private float volume = 1;

    private void Awake()
    {
        slider = GetComponent<Slider>();
    }

    public void OnSliderChanged()
    {
        volume = slider.value;
        AudioManager.Instance.ChangeVolume(volumeBus, volume);
    }
}
