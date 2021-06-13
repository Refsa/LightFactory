using Refsa.EventBus;
using UnityEngine;

public class Tooltip : MonoBehaviour
{
    [SerializeField] string title = "Placeholder...";
    [SerializeField, TextArea] string description = "Placeholder...";
    [SerializeField] Sprite icon;

    public string Title => title;
    public string Description => description;
    public Sprite Icon => icon;

    public void Show()
    {
        GlobalEventBus.Bus.Pub(new WorldItemTooltipShow(this));
    }

    public void Hide()
    {
        GlobalEventBus.Bus.Pub(new WorldItemTooltipHide());
    }
}