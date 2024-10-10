using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    //Manager Instances (Singletons)
    public static GameManager Instance;
    public static UIManager UIManagerInstance;
    public static ShopUIManager ShopUIManagerInstance;
    public static BuyingManager BuyingManagerInstance;
    public static InventoryUI InventoryUIInstance;

    //System Instances (Singletons)
    public static PlayerController PlayerControllerInstance;
    public static SuitSystem SuitSystemInstance;
    public static OxygenSystem OxygenSystemInstance;
    public static InventoryController InventoryControllerInstance;


    public bool isPaused { get; private set; } = false;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        //Setting all the Manager Instances
        Instance = this;
        UIManagerInstance = FindFirstObjectAndDestroyOthers<UIManager>();
        ShopUIManagerInstance = FindFirstObjectAndDestroyOthers<ShopUIManager>();
        BuyingManagerInstance = FindFirstObjectAndDestroyOthers<BuyingManager>();
        InventoryUIInstance = FindFirstObjectAndDestroyOthers<InventoryUI>();

        //Setting all the System Instances
        PlayerControllerInstance = FindFirstObjectAndDestroyOthers<PlayerController>();
        SuitSystemInstance = FindFirstObjectAndDestroyOthers<SuitSystem>();
        OxygenSystemInstance = FindFirstObjectAndDestroyOthers<OxygenSystem>();
        InventoryControllerInstance = FindFirstObjectAndDestroyOthers<InventoryController>();
    }

    private T FindFirstObjectAndDestroyOthers<T>()
    {
        GameObject[] objects = FindObjectsByType(typeof(T), FindObjectsSortMode.None) as GameObject[];
        if (objects.Length == 0)
        {
            return default(T);
        }
        if (objects.Length != 1)
        {
            for (int i = objects.Length; i >= 1; i++)
            {
                DestroyImmediate(objects[i].gameObject);
            }
        }
        return objects[0].gameObject.GetComponent<T>();
    } 

    private void Update()
    {
        //if (Input.GetKeyDown(KeyCode.Escape))
        //{
        //    isPaused = !isPaused;
        //}

        //if (Input.GetMouseButton(0) && Application.isFocused)
        //{
        //    isPaused = false;
        //}
    }
}
