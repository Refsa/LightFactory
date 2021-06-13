using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Refsa.EventBus;
using UnityEngine;

public interface ITicker
{
    int TickerPriority { get; }
    void Tick(int tick);
}

public struct RegisterTicker : IMessage
{
    public ITicker Ticker;

    public RegisterTicker(ITicker ticker)
    {
        Ticker = ticker;
    }
}

public struct UnregisterTicker : IMessage
{
    public ITicker Ticker;

    public UnregisterTicker(ITicker ticker)
    {
        Ticker = ticker;
    }
}

[DefaultExecutionOrder(-1000)]
public class Tick : MonoBehaviour
{
    static Tick instance;
    public static Tick Instance;

    [SerializeField] float tickRate = 0.25f;

    bool active;
    Coroutine tickRoutine;
    int currentTick;

    HashSet<ITicker> tickers;

    void Awake()
    {
        if (instance != null)
        {
            Destroy(this);
            return;
        }

        instance = this;
        tickers = new HashSet<ITicker>();

        GlobalEventBus.Bus.Sub<RegisterTicker>(OnRegisterTicker);
        GlobalEventBus.Bus.Sub<UnregisterTicker>(OnUnregisterTicker);

        active = true;

        OnEnable();
    }

    void OnEnable()
    {
        tickers = new HashSet<ITicker>();

        currentTick = 0;
        if (tickRoutine == null)
        {
            tickRoutine = StartCoroutine(TickTask());
        }
    }

    void OnDisable()
    {
        if (tickRoutine != null)
        {
            StopCoroutine(tickRoutine);
            tickRoutine = null;
        }
    }

    void OnDestroy()
    {
        instance = null;
    }

    IEnumerator TickTask()
    {
        while (true)
        {
            if (!active)
            {
                yield return new WaitWhile(() => !active);
            }

            foreach (var ticker in tickers.OrderBy(e => e.TickerPriority))
            {
                ticker.Tick(currentTick);
            }

            currentTick++;
            yield return new WaitForSeconds(tickRate);
        }
    }

    void OnRegisterTicker(RegisterTicker obj)
    {
        tickers.Add(obj.Ticker);
    }

    void OnUnregisterTicker(UnregisterTicker obj)
    {
        tickers.Remove(obj.Ticker);
    }
}


public static class TickerPriorities
{
    public const int LASER_SOURCE = 0;
    public const int LASER_COLLECTOR = 1;
    public const int LIGHT_COMBINER = 2;
}