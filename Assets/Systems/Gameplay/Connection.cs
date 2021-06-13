

public class Connection
{
    public LaserSource Src;
    public ILaserCollector Dest;
    public int LastConnectionTime;

    public Connection(LaserSource src, ILaserCollector dest)
    {
        Src = src;
        Dest = dest;
    }
}