using System.Collections;
using System.Collections.Generic;
using Refsa.EventBus;
using UnityEngine;

public class ThePrism : MonoBehaviour
{
    void Start()
    {
        GlobalEventBus.Bus.Pub(new ThePrismCollected());
    }
}

public struct ThePrismCollected : IMessage
{
    
}
