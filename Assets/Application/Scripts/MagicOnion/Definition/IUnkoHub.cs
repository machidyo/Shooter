public interface IUnkoHubReceiver
{
    void OnJoin(string name);
    void OnLeave(string name);
    void OnSendPosition(string name, float x, float y, float z);
    void OnShoot();
}