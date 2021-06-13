using System;
using System.Collections;
using System.Collections.Generic;
using Refsa.EventBus;
using UnityEngine;
using UnityEngine.UI;

public class LightItemUI : MonoBehaviour
{
    [SerializeField] bool listenEvent = true;

    [SerializeField] Sprite icon;

    [SerializeField] Image iconImage;
    [SerializeField] Text amountText;

    [SerializeField] LightLevel lightLevel;
    [SerializeField] LightColor lightColor;

    int currentAmount = 0;

    void Awake()
    {
        iconImage.sprite = icon;
        iconImage.color = lightColor.ToColor();

        if (listenEvent)
        {
            GlobalEventBus.Bus.Sub<LightInventoryChange>(OnLightInventoryChange);
        }
    }

    private void OnLightInventoryChange(LightInventoryChange obj)
    {
        if (obj.LightColor == lightColor && obj.LightLevel == lightLevel)
        {
            currentAmount += obj.Amount;
            amountText.text = $"{currentAmount}";
        }
    }

    public void SetAmount(int amount)
    {
        currentAmount = amount;
        amountText.text = $"{currentAmount}";
    }

    public void SetSprite(Sprite sprite, Color color)
    {
        iconImage.sprite = sprite;
        iconImage.color = color;
    }
}