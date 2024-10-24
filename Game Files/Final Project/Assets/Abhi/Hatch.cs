using System.Collections;
using UnityEngine;

public class Hatch : MonoBehaviour, IInteractable
{
    public Transform ramp;
    public float rotAngle, endRot, rotTime;
    private float startRot;
    private Vector3 rotation;

    public Transform from;
    public Transform to;
    public float speed = 0.01f;
    public float timeCount = 0.0f;

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
        while(ramp.transform.rotation != to.rotation)
        {
            ramp.transform.rotation = Quaternion.Lerp(from.rotation, to.rotation, timeCount * speed);
            timeCount = timeCount + Time.deltaTime;
            yield return null;
        }

        var temp = from;
        from = to;
        to = temp;
        timeCount = 0;
    }

}
