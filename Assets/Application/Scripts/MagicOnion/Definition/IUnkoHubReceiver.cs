using System.Threading.Tasks;
using MagicOnion;

public interface IUnkoHub : IStreamingHub<IUnkoHub, IUnkoHubReceiver>
{
    Task JoinAsync(string userName);
    Task LeaveAsync();
    Task SendMessageAsync(string message);
    Task SendPositionAsync(float x, float y, float z);
    Task ShootAsync();
}
