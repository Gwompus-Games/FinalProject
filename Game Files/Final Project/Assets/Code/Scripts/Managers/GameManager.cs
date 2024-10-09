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

    //Important Game Instances (Singletons)
    public static PlayerController PlayerControllerInstance;


    public bool isPaused { get; private set; } = false;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        UIManagerInstance = FindFirstObjectAndDestroyOthers<UIManager>();
        PlayerControllerInstance = FindFirstObjectAndDestroyOthers<PlayerController>();
        ShopUIManagerInstance = FindFirstObjectAndDestroyOthers<ShopUIManager>();
        BuyingManagerInstance = FindFirstObjectAndDestroyOthers<BuyingManager>();
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
                Destroy(objects[i].gameObject);
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
