using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UniRx;

public class MySceneManager : MonoBehaviour, IManager
{
    [SerializeField] private string mainScene;

    private static List<IManager> managers = new List<IManager>();
    
    private static readonly string startScene = "01_Start";
    private static readonly string endScene = "99_end";

    private static MySceneManager instance;

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(instance);
            Init();
        }
        else
        {
            Debug.Log("Destory!");
            Destroy(gameObject);
        }
    }
    
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space) || OVRInput.GetDown(OVRInput.Button.PrimaryIndexTrigger))
        {
            SceneManager.LoadScene(mainScene);
        }
        if (Input.GetKeyDown(KeyCode.E) && Input.GetKeyDown(KeyCode.LeftShift) && Input.GetKeyDown(KeyCode.LeftControl))
        {
            MoveEndScene();
        }
        if (Input.GetKeyDown(KeyCode.S) && Input.GetKeyDown(KeyCode.LeftShift) && Input.GetKeyDown(KeyCode.LeftControl))
        {
            BackStartScene();
        }
    }

    private static void InitAll()
    {
        Logger.Init();
        
        foreach (var manager in managers)
        {
            manager.Init();
        }
    }

    public void Init()
    {
        InitAll();
    }

    private static void BackStartScene()
    {
        InitAll();
        SceneManager.LoadScene(startScene);
    }

    public static void MoveEndScene()
    {
        SceneManager.LoadScene(endScene);
        Observable.Timer(TimeSpan.FromSeconds(3)).First().Subscribe(_ => BackStartScene());
    }
}
