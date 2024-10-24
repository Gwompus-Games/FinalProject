using System.Collections;
using UnityEngine;

public class Hatch : MonoBehaviour, IInteractable
{
    public Transform ramp;
    public float rotAngle, endRot, rotTime;
    private float startRot;
    private Vector3 rotation;

    private void Awake()
    {
        transform.tag = "Interactable";
        startRot = ramp.transform.localRotation.x;
        rotation = new Vector3(0, rotAngle, 0);
    }

    public void Interact()
    {
        //GameManager.Instance.GetManagedComponent<PlayerController>().TeleportPlayer(teleport.position);
        StartCoroutine(RotateRamp());
    }

    private IEnumerator RotateRamp()
    {
        Vector3 tempRot;
        if (endRot > startRot)
            tempRot = rotation;
        else
            tempRot = rotation * -1;

        while(ramp.transform.localRotation.x - endRot != 0)
        {
            ramp.Rotate(tempRot);
            yield return null;
        }
        
        Quaternion tempEuler = ramp.transform.localRotation;
        tempEuler.eulerAngles = new Vector3 (0, endRot, 0);
        ramp.transform.localRotation = tempEuler;

        var temp = startRot;
        startRot = endRot;
        endRot = temp;
    }
}
