using FMOD.Studio;
using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

public class Hatch : InteractableObject
{
    //public Transform teleport;
    public Animator anim;
    public EventInstance ramp, complete;

    public enum HatchState
    {
        OPEN,
        CLOSED,
        INBETWEEN
    }
    HatchState currentState = HatchState.CLOSED;

    protected override void Awake()
    {
        base.Awake();
    }
    
    protected override void Start()
    {
        base.Start();
        ramp = AudioManager.Instance.CreateEventInstance(FMODEvents.Instance.ramp);
        complete = AudioManager.Instance.CreateEventInstance(FMODEvents.Instance.landed);
    }

    public override void Interact()
    {
        if (GameManager.Instance.GetManagedComponent<Submarine>().landed)
        {
            //anim.SetBool("open", !anim.GetBool("open"));
            //AudioManager.Instance.OnClick();
            if(currentState == HatchState.INBETWEEN)
            {
                AudioManager.Instance.OnDenied();
                return;
            }
            StartCoroutine(RampAnimation());
        }
        else
        {
            AudioManager.Instance.OnDenied();
        }
    }

    public void CloseHatch()
    {
        anim.SetBool("open", false);
    }

    private IEnumerator RampAnimation()
    {
        AudioManager.Instance.OnClick();
        anim.SetBool("open", !anim.GetBool("open"));
        ramp.start();
        currentState = HatchState.INBETWEEN;

        yield return new WaitForSeconds(5.5f);
        ramp.stop(STOP_MODE.ALLOWFADEOUT);
        complete.start();

        if(anim.GetBool("open"))
        {
            currentState = HatchState.OPEN;
        }
        else
        {
            currentState = HatchState.CLOSED;
        }
    }
}
