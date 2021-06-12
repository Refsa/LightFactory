using System;
using System.Collections;
using System.Collections.Generic;
using Refsa.EventBus;
using UnityEngine;
using UnityEngine.UI;

public class BuildItemTooltipUI : MonoBehaviour
{
    [SerializeField] Text titleText;
    [SerializeField] Text descriptionText;

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

        var targetRT = obj.Target.GetComponent<RectTransform>();
        rectTransform.position = targetRT.position - new Vector3(0f, targetRT.sizeDelta.y * 2f, 0f);
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