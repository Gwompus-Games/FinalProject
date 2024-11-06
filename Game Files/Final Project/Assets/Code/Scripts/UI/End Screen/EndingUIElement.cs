using UnityEngine;

[RequireComponent(typeof(CanvasGroup))]
public class EndingUIElement : MonoBehaviour
{
    [field :SerializeField] public EndScreenManager.EndState endingState { get; private set; }
    private CanvasGroup _canvasGroup;

    private void Awake()
    {
        _canvasGroup = GetComponent<CanvasGroup>();
    }

    public void SetUIVisable(bool uiVisable)
    {
        if (uiVisable)
        {
            _canvasGroup.alpha = 1f;
        }
        else
        {
            _canvasGroup.alpha = 0f;
        }
    }
}
