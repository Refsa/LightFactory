using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlaceItemPreview : MonoBehaviour
{
    [SerializeField] SpriteRenderer previewRenderer;

    void Awake()
    {
        gameObject.SetActive(false);
    }

    public void Show(BuildItem buildItem)
    {
        previewRenderer.sprite = buildItem.Icon;
        gameObject.SetActive(true);
    }

    public void Hide()
    {
        gameObject.SetActive(false);
        previewRenderer.sprite = null;
    }

    public void SetColor(Color color)
    {
        previewRenderer.color = color;
    }
}
