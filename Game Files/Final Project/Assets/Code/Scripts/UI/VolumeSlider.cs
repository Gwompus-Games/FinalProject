using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class VolumeSlider : MonoBehaviour
{
    private Slider slider;
    public AudioManager.VolumeBus volumeBus;
    private float volume = 1;
    private bool updateVolume = false;

    private void Awake()
    {
        slider = GetComponent<Slider>();
    }

    //private void Update()
    //{
    //    if (!updateVolume)
    //        return;
    //    volume = slider.value;
    //    AudioManager.Instance.ChangeVolume(volumeBus, volume);
    //}

    public void UpdateVolume(bool update)
    {
        updateVolume = update;
        StartCoroutine(UpdateVolumeCoroutine());
    }

    private IEnumerator UpdateVolumeCoroutine()
    {
        while(updateVolume)
        {
            volume = slider.value;
            AudioManager.Instance.ChangeVolume(volumeBus, volume);
            yield return null;
        }
    }
}
