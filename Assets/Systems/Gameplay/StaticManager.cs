using System.Collections;
using System.Collections.Generic;
using Refsa.EventBus;
using UnityEngine;

[DefaultExecutionOrder(-100000)]
public class StaticManager : MonoBehaviour
{
    void Awake()
    {
        GlobalEventBus.Init();
        LaserSource.ClearGradients();
    }

    void OnDestroy()
    {

    }
}
