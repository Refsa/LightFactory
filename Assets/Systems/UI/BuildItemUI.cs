using System.Collections;
using System.Collections.Generic;
using Refsa.EventBus;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class BuildItemUI : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    [SerializeField] Image iconImage;

    BuildItem targetBuildItem;

    public BuildItem TargetBuildItem => targetBuildItem;

    public void Setup(BuildItem buildItem)
    {
        targetBuildItem = buildItem;
        iconImage.sprite = buildItem.Icon;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        GlobalEventBus.Bus.Pub(new BuildItemTooltipShow(this));
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        GlobalEventBus.Bus.Pub(new BuildItemTooltipHide());
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        GlobalEventBus.Bus.Pub(new BuildItemSelected(targetBuildItem));
    }
}

public struct BuildItemSelected : IMessage
{
    public readonly BuildItem BuildItem;

    public BuildItemSelected(BuildItem buildItem)
    {
        BuildItem = buildItem;
    }
}
