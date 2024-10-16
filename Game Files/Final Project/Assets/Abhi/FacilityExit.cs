using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public class FacilityExit : MonoBehaviour, IInteractable
{
    public Transform facilityExit;

    private void Awake()
    {
        transform.tag = "Interactable";
    }

    private void Start()
    {
        facilityExit = GameObject.Find("FacilityExit").transform;
    }

    public void Interact()
    {
        if(!facilityExit)
        {
            facilityExit = GameObject.Find("FacilityExit").transform;
        }
        print("interacted");
        //SceneManager.LoadScene(2);
        GameManager.PlayerControllerInstance.GetComponent<CharacterController>().enabled = false;
        if (facilityExit)
            GameManager.PlayerControllerInstance.transform.position = facilityExit.position;
        else
            print("No transform found");
        GameManager.PlayerControllerInstance.GetComponent<CharacterController>().enabled = true;
    }
}
