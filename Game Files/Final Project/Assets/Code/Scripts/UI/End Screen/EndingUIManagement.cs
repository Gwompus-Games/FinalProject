using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EndingUIManagement : MonoBehaviour
{
    private EndingUIElement[] _endingUIElements;

    private void OnEnable()
    {
        EndScreenManager.EndStateUpdate += SetStateVisable;
    }

    private void OnDisable()
    {
        EndScreenManager.EndStateUpdate -= SetStateVisable;
    }

    private void Start()
    {
        _endingUIElements = GetComponentsInChildren<EndingUIElement>();
        SetAllElementsNotVisable();
    }

    private void SetAllElementsNotVisable()
    {
        if (_endingUIElements.Length == 0)
        {
            return;
        }

        for (int e = 0; e < _endingUIElements.Length; e++)
        {
            _endingUIElements[e].SetUIVisable(false);
        }
    }

    public void SetStateVisable(EndScreenManager.EndState endState)
    {
        if (_endingUIElements.Length == 0)
        {
            return;
        }

        for (int e = 0; e < _endingUIElements.Length; e++)
        {
            _endingUIElements[e].SetUIVisable(false);
            if (_endingUIElements[e].endingState == endState)
            {
                _endingUIElements[e].SetUIVisable(true);
            }
        }
    }
}
