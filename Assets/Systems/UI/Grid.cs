using System;
using Refsa.EventBus;
using UnityEngine;

public class Grid : MonoBehaviour
{
    GameObject targetObject;

    void Awake()
    {
        GlobalEventBus.Bus.Sub<TransformHandleStatus>(OnTransformHandleStatus);
        GlobalEventBus.Bus.Sub<SelectionChanged>(OnSelectionChanged);

        gameObject.SetActive(false);
    }

    private void OnSelectionChanged(SelectionChanged obj)
    {
        targetObject = obj.Target;
    }

    private void OnTransformHandleStatus(TransformHandleStatus obj)
    {
        this.gameObject.SetActive(obj.State);
    }

    void Update()
    {
        if (targetObject == null) return;

        transform.position = targetObject.transform.position + Vector3.forward * 20f;
    }
}