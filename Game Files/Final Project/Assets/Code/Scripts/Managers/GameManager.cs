using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    //Manager Instances (Singletons)
    public static GameManager Instance;
    public static UIManager UIManagerInstance;
    public static ShopUIManager ShopUIManagerInstance;
    public static BuyingManager BuyingManagerInstance;
    public static InventoryUI InventoryUIInstance;

    //Input System
    public static CustomPlayerInput CustomPlayerInputInstance;

    //Game System Instances (Singletons)
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

        //Setting Input System
        CustomPlayerInputInstance = FindFirstObjectAndDestroyOthers<CustomPlayerInput>();

        //Setting all the System Instances
        PlayerControllerInstance = FindFirstObjectAndDestroyOthers<PlayerController>();
        SuitSystemInstance = FindFirstObjectAndDestroyOthers<SuitSystem>();
        OxygenSystemInstance = FindFirstObjectAndDestroyOthers<OxygenSystem>();
        InventoryControllerInstance = FindFirstObjectAndDestroyOthers<InventoryController>();
    }

    private T FindFirstObjectAndDestroyOthers<T>()
    {
        Object[] objects = FindObjectsByType(typeof(T), FindObjectsSortMode.None);
        GameObject[] gameObjects = new GameObject[objects.Length];
        int index = 0;
        for (int o = 0; o < objects.Length; o++)
        {
            if (objects[o].GameObject() != null)
            {
                gameObjects[index] = objects[o].GameObject();
                index++;
            }
        }
        if (gameObjects.Length == 0)
        {
            return default(T);
        }
        if (gameObjects.Length != 1)
        {
            for (int i = gameObjects.Length; i >= 1; i++)
            {
                DestroyImmediate(gameObjects[i].gameObject);
            }
        }
        return gameObjects[0].GetComponent<T>();
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
