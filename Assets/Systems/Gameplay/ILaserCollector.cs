using UnityEngine;

public interface ILaserCollector
{
    void Notify(Color color, LightPacket lightPacket);
    void NotifyConnected(Connection connection);
    void NotifyDisconnected(Connection connection);
}