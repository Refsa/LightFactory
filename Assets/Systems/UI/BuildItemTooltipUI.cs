using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Refsa.EventBus;
using UnityEngine;
using UnityEngine.UI;

public class BuildItemTooltipUI : MonoBehaviour
{
    [SerializeField] Text titleText;
    [SerializeField] Text descriptionText;

    [SerializeField] RectTransform costContainer;

    [SerializeField] Sprite levelOneSprite;
    [SerializeField] Sprite levelTwoSprite;
    [SerializeField] Sprite levelThreeSprite;

    RectTransform rectTransform;

    void Awake()
    {
        rectTransform = GetComponent<RectTransform>();

        GlobalEventBus.Bus.Sub<BuildItemTooltipShow>(OnShowTooltip);
        GlobalEventBus.Bus.Sub<BuildItemTooltipHide>(OnHideTooltip);

        this.gameObject.SetActive(false);
    }

    private void OnHideTooltip(BuildItemTooltipHide obj)
    {
        this.gameObject.SetActive(false);
    }

    private void OnShowTooltip(BuildItemTooltipShow obj)
    {
        this.gameObject.SetActive(true);

        titleText.text = obj.Target.TargetBuildItem.name;
        descriptionText.text = obj.Target.TargetBuildItem.Tooltip;

        var costs = obj.Target.TargetBuildItem.Cost;
        for (int i = 0; i < 6; i++)
        {
            var costUI = costContainer.GetChild(i).GetComponent<LightItemUI>();
            if (i >= costs.Count) costUI.gameObject.SetActive(false);
            else
            {
                costUI.gameObject.SetActive(true);

                costUI.SetAmount(costs[i].Amount);

                var sprite = costs[i].LightMeta.LightLevel switch
                {
                    LightLevel.One => levelOneSprite,
                    LightLevel.Two => levelTwoSprite,
                    LightLevel.Three => levelThreeSprite,
                    _ => null,
                };

                costUI.SetSprite(sprite, costs[i].LightMeta.LightColor.ToColor());
            }
        }

        var targetRT = obj.Target.GetComponent<RectTransform>();
        rectTransform.position = targetRT.position - new Vector3(0f, targetRT.sizeDelta.y * 2.5f, 0f);
        rectTransform.position = new Vector3(25f, rectTransform.position.y, 0f);
    }
}

public struct BuildItemTooltipShow : IMessage
{
    public BuildItemUI Target;

    public BuildItemTooltipShow(BuildItemUI target)
    {
        Target = target;
    }
}

public struct BuildItemTooltipHide : IMessage
{

}