using System;
using System.Collections.Generic;
using System.Linq;
using Refsa.EventBus;
using UnityEngine;

public class LightInventory : MonoBehaviour
{
    static LightInventory instance;
    public static LightInventory Instance => instance;

    [SerializeField] List<LightCurrency> startingInventory;

    Dictionary<LightMeta, LightCurrency> currencies;

    void Awake()
    {
        if (instance != null)
        {
            Destroy(this);
            return;
        }

        instance = this;

        currencies = new Dictionary<LightMeta, LightCurrency>();

        foreach (var item in startingInventory)
        {
            if (!currencies.ContainsKey(item.LightMeta))
            {
                currencies.Add(item.LightMeta, new LightCurrency(item.LightMeta, item.Amount));
            }
            else
            {
                currencies[item.LightMeta].Amount += item.Amount;
            }
        }
    }

    void Start()
    {
        foreach (var item in currencies.Values)
        {
            GlobalEventBus.Bus.Pub(new LightInventoryChange(item.LightMeta.LightLevel, item.LightMeta.LightColor, item.Amount));
        }

        GlobalEventBus.Bus.Sub<LightInventoryChange>(OnLightInventoryChange);
    }

    private void OnLightInventoryChange(LightInventoryChange obj)
    {
        var key = currencies.Keys.Where(e => e.LightColor == obj.LightColor && e.LightLevel == obj.LightLevel).FirstOrDefault();
        if (key == null)
        {
            key = new LightMeta(obj.LightLevel, obj.LightColor);
            currencies.Add(key, new LightCurrency(key, 0));
        }

        currencies[key].Amount += obj.Amount;
    }

    public bool CanAfford(List<LightCurrency> costs)
    {
        foreach (var cost in costs)
        {
            var key = currencies.Keys.Where(e => e.LightColor == cost.LightMeta.LightColor && e.LightLevel == cost.LightMeta.LightLevel).FirstOrDefault();
            if (key == null) return false;

            if (currencies[key].Amount < cost.Amount)
            {
                return false;
            }
        }

        return true;
    }
}

public struct LightInventoryChange : IMessage
{
    public readonly LightLevel LightLevel;
    public readonly LightColor LightColor;
    public readonly int Amount;

    public LightInventoryChange(LightLevel lightLevel, LightColor lightColor, int amount)
    {
        LightLevel = lightLevel;
        LightColor = lightColor;
        Amount = amount;
    }
}