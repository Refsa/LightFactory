using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightPacketVisual : MonoBehaviour
{
    [SerializeField] Sprite levelOne;
    [SerializeField] Sprite levelTwo;
    [SerializeField] Sprite levelThree;
    [SerializeField] Sprite levelFour;

    SpriteRenderer spriteRenderer;

    void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    public void SetLevel(LightLevel level)
    {
        switch (level)
        {
            case LightLevel.One:
                spriteRenderer.sprite = levelOne;
                break;
            case LightLevel.Two:
                spriteRenderer.sprite = levelTwo;
                break;
            case LightLevel.Three:
                spriteRenderer.sprite = levelThree;
                break;
            case LightLevel.Four:
                spriteRenderer.sprite = levelFour;
                break;
        }
    }

    public void SetColor(Color color)
    {
        spriteRenderer.color = color;
    }
}
