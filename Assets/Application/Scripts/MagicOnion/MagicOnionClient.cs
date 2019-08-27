using System;
using UnityEngine;
using Grpc.Core;
using MagicOnion.Client;
using UniRx;
using UniRx.Async;
using UniRx.Triggers;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;

public class MagicOnionClient : MonoBehaviour, IUnkoHubReceiver
{
    [SerializeField] private GameObject unkoman;
    [SerializeField] private GameObject ponponUnko;

    public ReactiveProperty<bool> IsJoin = new ReactiveProperty<bool>();
    public Action ShootAction;

    private Channel channel;
    private IUnkoHub unkoHub;

    void Start()
    {
        IsJoin.Value = false;

        channel = ChannelController.GetChannel(false);
        unkoHub = StreamingHubClient.Connect<IUnkoHub, IUnkoHubReceiver>(channel, this);
    }

    void Update()
    {
    }

    private async void OnDestroy()
    {
        await unkoHub.DisposeAsync();
        await channel.ShutdownAsync();
    }

    public async void Join()
    {
        if (channel == null)
        {
            Debug.Log("Channel is null, so recreation.");
            channel = ChannelController.GetChannel(true);
        }
        if (unkoHub == null)
        {
            Debug.Log("Hub is null, so recreation.");
            unkoHub = StreamingHubClient.Connect<IUnkoHub, IUnkoHubReceiver>(channel, this);
        }
        
#if UNITY_EDITOR
        await unkoHub.JoinAsync("Editor");
#else
        await unkoHub.JoinAsync("Oculus Quest");
#endif
        IsJoin.Value = true;
        Debug.Log("You joined in the room");
    }

    public async void Leave()
    {
        await unkoHub.LeaveAsync();
        IsJoin.Value = false;
        Debug.Log("You left in the room");
    }

    public async void SendPosition(float x, float y, float z)
    {
        await unkoHub.SendPositionAsync(x, y, z);
    }

    public async void Shoot()
    {
        await unkoHub.ShootAsync();
        Debug.Log($"Shoot");
    }
    
    public void OnJoin(string name)
    {
        Debug.Log($"{name} joined in the room.");
    }

    public void OnLeave(string name)
    {
        Debug.Log($"{name} left in the room.");
    }

    public void OnSendPosition(string name, float x, float y, float z)
    {
        if(name != "Oculus Quest") {return;}
        unkoman.transform.position = new Vector3(x, y, z);
    }

    public void OnShoot()
    {
        ShootAction();
    }
}
