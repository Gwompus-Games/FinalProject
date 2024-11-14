using UnityEngine;
using UnityEngine.UI;

public class VideoSettings : MonoBehaviour
{
    public Button low, medium, high;

    public void Low()
    {
        if (!low.interactable)
            return;
        QualitySettings.SetQualityLevel(0);
        EnableButtons(low);
    }

    public void Medium()
    {
        if (!medium.interactable)
            return;
        QualitySettings.SetQualityLevel(1);
        EnableButtons(medium);
    }

    public void High()
    {
        if (!high.interactable)
            return;
        QualitySettings.SetQualityLevel(2);
        EnableButtons(high);
    }

    private void EnableButtons(Button button)
    {
        AudioManager.Instance.OnClick();

        low.interactable = true;
        medium.interactable = true;
        high.interactable = true;

        button.interactable = false;
    }
}
