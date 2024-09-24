using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class OxygenSystem : MonoBehaviour
{
    public static OxygenSystem INSTANCE;

    [SerializeField] private List<OxygenTank> oxygenTanks = new List<OxygenTank>();
    private int activeOxygenTank = 0;
    private Dictionary<MonoBehaviour, float> oxygenDrainMultipliers = new Dictionary<MonoBehaviour, float>();

    private void Awake()
    {
        if (INSTANCE != null)
        {
            Destroy(gameObject);
            return;
        }
        INSTANCE = this;
    }

    private void Start()
    {
        
    }

    private void Update()
    {
        
    }

    private void SwapOxygenTank()
    {
        int selectedTank = 0;
        if (oxygenTanks[selectedTank].containsOxygen)
        {

        }
        for ()
        {

        }
    }
    private void SwapOxygenTank(int tank)
    {

    }
}
