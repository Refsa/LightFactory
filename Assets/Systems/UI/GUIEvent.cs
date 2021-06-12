using System.Collections;
using System.Collections.Generic;
using Refsa.EventBus;
using UnityEngine;
using UnityEngine.EventSystems;

public class GUIEvent : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler
{
    [SerializeField] string identifier;

    public void OnPointerDown(PointerEventData eventData)
    {
        GlobalEventBus.Bus.Pub(new ElementClicked(identifier));
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        GlobalEventBus.Bus.Pub(new ElementHover(identifier, true));
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        GlobalEventBus.Bus.Pub(new ElementHover(identifier, false));
    }
}

public struct ElementHover : IMessage
{
    public string Identifier;
    public bool State;

    public ElementHover(string identifier, bool state)
    {
        Identifier = identifier;
        State = state;
    }
}

public struct ElementClicked : IMessage
{
    public string Identifier;

    public ElementClicked(string identifier)
    {
        Identifier = identifier;
    }
}
