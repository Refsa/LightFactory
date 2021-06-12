using System.Collections;
using System.Collections.Generic;
using Refsa.EventBus;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class BuildItemUI : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    [SerializeField] Image iconImage;
    [SerializeField] Image backgroundImage;

    BuildItem targetBuildItem;
    bool isSelected;

    public BuildItem TargetBuildItem => targetBuildItem;

    public void Setup(BuildItem buildItem)
    {
        targetBuildItem = buildItem;
        iconImage.sprite = buildItem.Icon;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        GlobalEventBus.Bus.Pub(new BuildItemTooltipShow(this));
        backgroundImage.color = Color.gray;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        GlobalEventBus.Bus.Pub(new BuildItemTooltipHide());

        if (!isSelected)
        {
            backgroundImage.color = Color.white;
        }
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        GlobalEventBus.Bus.Pub(new BuildItemSelected(targetBuildItem, this));
    }

    public void Select()
    {
        isSelected = true;
        backgroundImage.color = Color.green;
    }

    public void Deselect()
    {
        isSelected = false;
        backgroundImage.color = Color.white;
    }
}

public struct BuildItemSelected : IMessage
{
    public readonly BuildItem BuildItem;
    public readonly BuildItemUI BuildItemUI;

    public BuildItemSelected(BuildItem buildItem, BuildItemUI buildItemUI)
    {
        BuildItem = buildItem;
        BuildItemUI = buildItemUI;
    }
}
