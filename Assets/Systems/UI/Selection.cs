using System;
using System.Collections;
using System.Collections.Generic;
using Refsa.EventBus;
using UnityEngine;

public class Selection : MonoBehaviour
{
    int targetLayerMask;
    GameObject selectedGameObject;
    bool selectionLocked;
    bool selectionChanged;

    [AutoBind] new Camera camera;

    void Awake()
    {
        targetLayerMask = 1 << LayerMask.NameToLayer("Solid");

        GlobalEventBus.Bus.Sub<SelectionLock>(OnSelectionLock);
        GlobalEventBus.Bus.Sub<SelectionSet>(OnSelectionSet);
    }

    private void OnSelectionSet(SelectionSet obj)
    {
        selectedGameObject = obj.Target;

        if (selectedGameObject != null)
        {
            if (!selectedGameObject.HasTag("Selectable") && !selectedGameObject.HasTagInParent("Selectable"))
            {
                selectedGameObject = null;
            }
        }

        selectionChanged = true;
    }

    void OnSelectionLock(SelectionLock obj)
    {
        selectionLocked = obj.Lock;
    }

    void Update()
    {
        if (selectionChanged)
        {
            GlobalEventBus.Bus.Pub(new SelectionChanged(selectedGameObject));
            selectionChanged = false;
            return;
        }

        bool hadSelection = selectedGameObject != null;

        if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            var mouseRay = camera.ScreenPointToRay(Input.mousePosition);
            mouseRay.origin.Scale(new Vector3(1, 1, 0));

            var hit = Physics2D.OverlapCircle(mouseRay.origin, 0.25f, targetLayerMask);

            if (hit != null && (hit.gameObject.HasTag("Selectable") || hit.gameObject.HasTagInParent("Selectable")))
            {
                if (!hadSelection || selectedGameObject != hit.gameObject)
                {
                    selectionChanged = true;
                }
                selectedGameObject = hit.gameObject;

            }
            else if (!selectionLocked)
            {
                selectedGameObject = null;
                selectionChanged = true;
            }
        }

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            selectedGameObject = null;
            selectionChanged = true;
        }

        if (selectionChanged)
        {
            GlobalEventBus.Bus.Pub(new SelectionChanged(selectedGameObject));
            selectionChanged = false;
        }
    }
}

public struct SelectionChanged : IMessage
{
    public GameObject Target;
    public bool HasSelection => Target != null;

    public SelectionChanged(GameObject target)
    {
        Target = target;
    }
}

public struct SelectionLock : IMessage
{
    public bool Lock;

    public SelectionLock(bool locked)
    {
        Lock = locked;
    }
}

public struct SelectionSet : IMessage
{
    public readonly GameObject Target;

    public SelectionSet(GameObject target)
    {
        this.Target = target;
    }
}
