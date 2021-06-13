using System;
using System.Collections;
using System.Collections.Generic;
using Refsa.EventBus;
using UnityEngine;

public class TransformHandle : MonoBehaviour
{
    [SerializeField] PositionHandle positionHandle;
    [SerializeField] RotationHandle rotationHandle;
    [SerializeField] DeletaHandle deleteHandle;

    public bool Active => positionHandle.Active || rotationHandle.Active;

    GameObject selected;
    GameObject rotateable;
    GameObject movable;
    GameObject deleteable;

    void Awake()
    {
        GlobalEventBus.Bus.Sub<SelectionChanged>(OnSelectionChanged);
        GlobalEventBus.Bus.Sub<TransformHandleToggle>(OnTransformHandleToggle);
        gameObject.SetActive(false);

        positionHandle.mouseEntered += () => GlobalEventBus.Bus.Pub(new SelectionLock(true));
        rotationHandle.mouseEntered += () => GlobalEventBus.Bus.Pub(new SelectionLock(true));
        deleteHandle.mouseEntered += () => GlobalEventBus.Bus.Pub(new SelectionLock(true));

        positionHandle.mouseLeft += () => GlobalEventBus.Bus.Pub(new SelectionLock(false));
        rotationHandle.mouseLeft += () => GlobalEventBus.Bus.Pub(new SelectionLock(false));
        deleteHandle.mouseLeft += () => GlobalEventBus.Bus.Pub(new SelectionLock(false));
    }

    private void OnTransformHandleToggle(TransformHandleToggle obj)
    {
        gameObject.SetActive(obj.State);
    }

    void Update()
    {
        if (selected == null) return;

        transform.position = selected.transform.position + Vector3.forward * -5f;

        if (movable != null)
        {
            positionHandle.SetData(movable);
            positionHandle.Handle(movable);
        }

        if (rotateable != null)
        {
            rotationHandle.SetData(rotateable);
            rotationHandle.Handle(rotateable);
        }

        if (deleteHandle != null)
        {
            deleteHandle.Handle(deleteable);
        }
    }

    private void OnSelectionChanged(SelectionChanged obj)
    {
        if (obj.HasSelection)
        {
            selected = obj.Target;
            SetData(selected);
            gameObject.SetActive(true);
        }
        else
        {
            selected = null;
            movable = null;
            rotateable = null;
            gameObject.SetActive(false);

            GlobalEventBus.Bus.Pub(new TransformHandleStatus(false));
        }
    }

    public void SetData(GameObject targetObject)
    {
        if (targetObject.HasTag("Movable"))
        {
            movable = targetObject;
        }
        else if (targetObject.HasTagInParent("Movable"))
        {
            movable = targetObject.transform.parent.gameObject;
        }
        else
        {
            movable = null;
        }

        if (targetObject.HasTag("Rotateable"))
        {
            rotateable = targetObject;
        }
        else if (targetObject.HasTagInParent("Rotateable"))
        {
            rotateable = targetObject.transform.parent.gameObject;
        }
        else
        {
            rotateable = null;
        }

        if (targetObject.HasTag("Deleteable"))
        {
            deleteable = targetObject;
        }
        else if (targetObject.HasTagInParent("Deleteable"))
        {
            deleteable = targetObject.transform.parent.gameObject;
        }
        else
        { 
            deleteable = null;
        }

        positionHandle.gameObject.SetActive(movable != null);
        rotationHandle.gameObject.SetActive(rotateable != null);

        if (movable != null)
        {
            positionHandle.SetData(movable);
        }
        if (rotateable != null)
        {
            rotationHandle.SetData(rotateable);
        }

        if (movable == null && rotateable == null)
        {
            GlobalEventBus.Bus.Pub(new TransformHandleStatus(false));
        }
        else
        {
            GlobalEventBus.Bus.Pub(new TransformHandleStatus(true));
        }
    }
}

public struct TransformHandleStatus : IMessage
{
    public bool State;

    public TransformHandleStatus(bool state)
    {
        State = state;
    }
}

public struct TransformHandleToggle : IMessage
{
    public bool State;

    public TransformHandleToggle(bool state)
    {
        State = state;
    }
}