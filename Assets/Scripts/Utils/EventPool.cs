using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public static class EventPool
{
    static public UnityEvent BodyDragged = new UnityEvent();
    static public UnityEvent<Vector3> BodyFinishedDragging = new UnityEvent<Vector3>();
    static public UnityEvent<int> finishedFixing = new UnityEvent<int>();
}
