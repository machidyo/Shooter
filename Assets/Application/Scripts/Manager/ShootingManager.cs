using System;
using System.Collections;
using System.Collections.Generic;
using UniRx;
using UniRx.Triggers;
using UnityEngine;
using UnityEngine.Serialization;

public class ShootingManager : MonoBehaviour, IManager
{
    public enum Status
    {
        Initializing,
        Initialized,
        Playing,
        Result,
        Finish,
    }

    [SerializeField] private Player player;
    [SerializeField] private GameObject enemy;
    
    public ReactiveProperty<Status> GameStatus = new ReactiveProperty<Status>();
    public ReactiveProperty<int> Timer = new ReactiveProperty<int>();
    public ReactiveProperty<int> Score = new ReactiveProperty<int>();
    public ReactiveProperty<string> Message = new ReactiveProperty<string>();

    void Start()
    {
        Init();
        StartGame();
    }

    void Update()
    {
        
    }

    public void Init()
    {
        GameStatus.Value = Status.Initializing;
        
        Timer.Value = 10;
        Score.Value = 0;
        Message.Value = "";
        
        GameStatus.Value = Status.Initialized;
    }

    private void StartGame()
    {
        Observable.Interval(TimeSpan.FromSeconds(1))
            .Where(_ => GameStatus.Value == Status.Playing)
            .Subscribe(_ => Timer.Value--).AddTo(this);

        Observable.Interval(TimeSpan.FromSeconds(0.5))
            .Where(_ => GameStatus.Value == Status.Playing)
            .Subscribe(_ => CreateEnemy()).AddTo(this);

        Message.Value = "GAME START";
        Observable.Timer(TimeSpan.FromSeconds(1))
            .First()
            .Subscribe(_ => Message.Value = "").AddTo(this);

        Timer.DistinctUntilChanged()
            .Where(timer => timer == 0)
            .Where(_ => GameStatus.Value == Status.Playing)
            .Subscribe(_ => ShowResult())
            .AddTo(this);

        player.HitPoint.DistinctUntilChanged()
            .Where(hp => hp == 0)
            .Subscribe(_ => ShowResult())
            .AddTo(this);

        GameStatus.Value = Status.Playing;
    }
    private void CreateEnemy()
    {
        var x = UnityEngine.Random.Range(-5f, 5f);
        var z = UnityEngine.Random.Range(0f, 5f);
        var position = new Vector3(x, 0, z);
        var enemy = Instantiate(this.enemy, position, Quaternion.identity);
        enemy.OnDestroyAsObservable().Subscribe(_ => Score.Value++);
    }

    private void ShowResult()
    {
        if (Timer.Value == 0)
        {
            Message.Value = $"You killed {Score.Value} enemies.";
        }
        else if (player.HitPoint.Value == 0)
        {
            Message.Value = "GAME OVER";
        }
        
        GameStatus.Value = Status.Result;
        Observable.Timer(TimeSpan.FromSeconds(5)).Subscribe(_ => FinishGame()).AddTo(this);
    }

    private void FinishGame()
    {
        GameStatus.Value = Status.Finish;
        MySceneManager.MoveEndScene();
    }
}
