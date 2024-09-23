using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class OxygenSystem : MonoBehaviour
{
    public static OxygenSystem INSTANCE;

    [SerializeField] private List<OxygenTank> oxygenTanks = new List<OxygenTank>();
    private int activeOxygenTank = 0;

    private void Awake()
    {
        if (INSTANCE != null)
        {
            Destroy(gameObject);
            return;
        }
        INSTANCE = this;
    }

    private void Update()
    {
        
    }

}
