using UnityEngine;
using Grpc.Core;
using MagicOnion.Client;
using UniRx;
using UniRx.Triggers;
using UnityEngine.Serialization;

public class MagicOnionClient : MonoBehaviour, IUnkoHubReceiver
{
    [SerializeField] private GameObject unkoman;
    [SerializeField] private GameObject ponponUnko;

    public ReactiveProperty<bool> IsJoin = new ReactiveProperty<bool>();

    private string serverIp = "10.32.154.25";

    private Channel channel;
    private IUnkoHub unkoHub;

    void Start()
    {
        IsJoin.Value = false;
        
        channel = new Channel($"{serverIp}:12345", ChannelCredentials.Insecure);
        unkoHub = StreamingHubClient.Connect<IUnkoHub, IUnkoHubReceiver>(channel, this);
    }

    private async void OnDestroy()
    {
        await unkoHub.DisposeAsync();
        await channel.ShutdownAsync();
    }

    public async void Join()
    {
#if UNITY_EDITOR
        await unkoHub.JoinAsync("Editor");
#else
        await chatHub.JoinAsync("Oculus Quest");
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
        Ponpon();
    }
    private void Ponpon()
    {
        var pos = transform.position + new Vector3(-0.8f, 1f, 1.5f);
        var obj = Instantiate(ponponUnko, pos, new Quaternion());

        var x = (Random.value - 0.5f) * 2f;
        obj.GetComponent<Rigidbody>().AddForce(new Vector3(x, 1f, 1f) * 5, ForceMode.Impulse);

        obj.OnCollisionEnterAsObservable().Subscribe(_ =>
        {
            Debug.Log($"Hit to {_.transform.name}");
            if (_.transform.name.ToLower() == "plane") return;

            Destroy(obj, 1);
        });
    }
}
