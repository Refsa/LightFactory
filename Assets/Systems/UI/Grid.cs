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

        GlobalEventBus.Bus.Sub<GridEnable>(OnGridEnable);
        GlobalEventBus.Bus.Sub<GridDisable>(OnGridDisable);

        gameObject.SetActive(false);
    }

    private void OnGridDisable(GridDisable obj)
    {
        this.gameObject.SetActive(false);
    }

    private void OnGridEnable(GridEnable obj)
    {
        targetObject = obj.Target;
        this.gameObject.SetActive(true);
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

public struct GridEnable : IMessage
{
    public readonly GameObject Target;

    public GridEnable(GameObject target)
    {
        Target = target;
    }
}

public struct GridDisable : IMessage
{

}