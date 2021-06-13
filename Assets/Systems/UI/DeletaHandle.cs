using System;
using System.Collections;
using System.Collections.Generic;
using Refsa.EventBus;
using UnityEngine;

public class DeletaHandle : Handle
{
    bool wasClicked;

    protected override Vector3 defaultPosition => transform.parent.position + transform.parent.up * 1f;

    void Awake()
    {
        mouse0Clicked += OnMouse0Clicked;
    }

    void OnMouse0Clicked()
    {
        wasClicked = true;
    }

    public void Handle(GameObject gameObject)
    {
        if (wasClicked)
        {
            GlobalEventBus.Bus.Pub(new SelectionChanged(null));
            GameObject.Destroy(gameObject);
            wasClicked = false;
        }
    }
}
