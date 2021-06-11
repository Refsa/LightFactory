

public static class Pools
{
    static readonly LightPacketPooler lightPacketPooler;
    public static LightPacketPooler LightPacketPooler => lightPacketPooler;

    static Pools()
    {
        lightPacketPooler = new LightPacketPooler();
    }
}