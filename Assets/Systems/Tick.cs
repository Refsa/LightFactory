using System;
using System.Collections;
using System.Collections.Generic;
using Refsa.EventBus;
using UnityEngine;

public interface ITicker
{
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
        tickRoutine = StartCoroutine(TickTask());
    }

    void OnEnable()
    {
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

            foreach (var ticker in tickers)
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
