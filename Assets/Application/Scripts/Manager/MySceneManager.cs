using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity;
using UnityEngine.SceneManagement;

public class MySceneManager : MonoBehaviour, IManager
{
    [SerializeField] private string mainScene;
    [SerializeField] private List<IManager> managers = new List<IManager>();
    
    private static readonly string startScene = "01_Start";
    private static readonly string endScene = "99_end";
    
    void Awake()
    {
        DontDestroyOnLoad(this);
        
        Logger.Init();
    }
    
    void Start()
    {
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            SceneManager.LoadScene(mainScene);
        }
        if (Input.GetKeyDown(KeyCode.S))
        {
            Init();
            SceneManager.LoadScene(startScene);
        }
        if (Input.GetKeyDown(KeyCode.E))
        {
            SceneManager.LoadScene(endScene);
        }
    }

    public void Init()
    {
        foreach (var manager in managers)
        {
            manager.Init();
        }
    }
}
