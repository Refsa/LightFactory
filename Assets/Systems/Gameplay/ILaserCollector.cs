using UnityEngine;

public interface ILaserCollector
{
    void Notify(Color color);
    void NotifyConnected(Connection connection);
    void NotifyDisconnected(Connection connection);
}