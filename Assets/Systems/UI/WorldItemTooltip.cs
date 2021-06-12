using System;
using Refsa.EventBus;
using UnityEngine;
using UnityEngine.UI;

public class WorldItemTooltip : MonoBehaviour
{
    [SerializeField] Text titleText;
    [SerializeField] Text descriptionText;
    [SerializeField] Image iconImage;

    RectTransform rectTransform;
    
    Tooltip lastActiveTooltip;

    void Awake()
    {
        rectTransform = GetComponent<RectTransform>();

        GlobalEventBus.Bus.Sub<WorldItemTooltipShow>(OnShowTooltip);
        GlobalEventBus.Bus.Sub<WorldItemTooltipHide>(OnHideTooltip);
        GlobalEventBus.Bus.Sub<SelectionChanged>(OnSelectionChanged);

        this.gameObject.SetActive(false);
    }

    private void OnSelectionChanged(SelectionChanged obj)
    {
        if (obj.HasSelection)
        {
            var tooltip = obj.Target.GetComponent<Tooltip>();
            if (tooltip == null)
            {
                tooltip = obj.Target.GetComponentInParent<Tooltip>();
            }

            if (tooltip == null) return;

            tooltip.Show();
            lastActiveTooltip = tooltip;
        }
        else if (lastActiveTooltip != null)
        {
            lastActiveTooltip.Hide();
            lastActiveTooltip = null;
        }
    }

    private void OnHideTooltip(WorldItemTooltipHide obj)
    {
        gameObject.SetActive(false);
    }

    private void OnShowTooltip(WorldItemTooltipShow obj)
    {
        gameObject.SetActive(true);

        titleText.text = obj.Target.Title;
        descriptionText.text = obj.Target.Description;
        iconImage.sprite = obj.Target.Icon;
    }
}

public struct WorldItemTooltipShow : IMessage
{
    public Tooltip Target;

    public WorldItemTooltipShow(Tooltip target)
    {
        Target = target;
    }
}

public struct WorldItemTooltipHide : IMessage
{

}