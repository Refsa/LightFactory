using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PositionHandle : Handle
{
    protected override Vector3 defaultPosition => transform.parent.position + transform.parent.up * offset;
    public override Vector2 Value => dragDelta;

    float offset = -0.25f;

    public void SetData(GameObject targetObject)
    {
        offset = targetObject.transform.localScale.y * -0.6f;
    }
}
