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

    [AutoBind] new Camera camera;

    void Awake()
    {
        targetLayerMask = 1 << LayerMask.NameToLayer("Solid");

        GlobalEventBus.Bus.Sub<SelectionLock>(OnSelectionLock);
    }

    private void OnSelectionLock(SelectionLock obj)
    {
        selectionLocked = obj.Lock;
    }

    void Update()
    {
        bool hadSelection = selectedGameObject != null;
        bool selectionChanged = false;

        if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            var mouseRay = camera.ScreenPointToRay(Input.mousePosition);
            mouseRay.origin.Scale(new Vector3(1, 1, 0));

            var hit = Physics2D.OverlapCircle(mouseRay.origin, 0.25f, targetLayerMask);

            if (hit != null && (hit.gameObject.HasTagInParent("Selectable") || hit.gameObject.HasTag("Selectable")))
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
            if (selectedGameObject == null)
            {
                GlobalEventBus.Bus.Pub(new SelectionChanged(null));
            }
            else
            {
                GlobalEventBus.Bus.Pub(new SelectionChanged(selectedGameObject));
            }
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

    public SelectionLock(bool @lock)
    {
        Lock = @lock;
    }
}
