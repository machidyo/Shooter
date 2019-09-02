using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UniRx;

public class MySceneManager : MonoBehaviour, IManager
{
    [SerializeField] private string mainScene;
    [SerializeField] private List<IManager> managers = new List<IManager>();
    
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
            Destroy(gameObject);
        }
    }
    
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            SceneManager.LoadScene(mainScene);
        }
        if (Input.GetKeyDown(KeyCode.S))
        {
            BackStartScene();
        }
        if (Input.GetKeyDown(KeyCode.E))
        {
            MoveEndScene();
        }
    }

    public void Init()
    {
        Logger.Init();
        
        foreach (var manager in managers)
        {
            manager.Init();
        }
    }

    private void BackStartScene()
    {
        Init();
        SceneManager.LoadScene(startScene);
    }

    private void MoveEndScene()
    {
        SceneManager.LoadScene(endScene);
        Observable.Timer(TimeSpan.FromSeconds(3)).First().Subscribe(_ => BackStartScene());
    }
}
