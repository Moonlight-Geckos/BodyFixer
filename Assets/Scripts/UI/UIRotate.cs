using System.Collections;
using UnityEngine;
using UnityEngine.Animations.Rigging;
using UnityEngine.EventSystems;

public class UIRotate : MonoBehaviour
{
    [SerializeField]
    private string targetTag = "";
    [SerializeField]
    private Vector3 desiredTargetRotation;
    [SerializeField]
    private bool match_X_Rotation;
    [SerializeField]
    private bool match_Y_Rotation;
    [SerializeField]
    private bool match_Z_Rotation;

    private Camera mainCamera; // Used for calculations
    private Transform target;
    private Transform joint;
    private Vector3 maxRotationDistance;
    private bool isDragged = false;
    private Vector3 startUI;
    private Vector3 startTarget;
    private void Awake()
    {
        EventTrigger eventTrigger = GetComponent<EventTrigger>();
        EventTrigger.Entry dragEntry = new EventTrigger.Entry();
        EventTrigger.Entry dropEntry = new EventTrigger.Entry();
        dragEntry.eventID = EventTriggerType.Drag;
        dropEntry.eventID = EventTriggerType.Drop;
        dragEntry.callback.AddListener(DragHandler);
        dropEntry.callback.AddListener(DropHandler);
        eventTrigger.triggers.Add(dragEntry);
        eventTrigger.triggers.Add(dropEntry);

        mainCamera = Camera.main;
        target = GameObject.FindGameObjectWithTag(targetTag).transform;
        joint = target.parent.GetComponent<MultiAimConstraint>().data.constrainedObject;
    }
    IEnumerator Start()
    {
        yield return new WaitForEndOfFrame();
        EventPool.BodyDragged.AddListener(ResetPositionOnJoint);
        ResetPositionOnJoint();
        Vector3 curRotation = joint.localEulerAngles;
        curRotation.x -= curRotation.x > 180 ? 360 : 0;
        curRotation.y -= curRotation.y > 180 ? 360 : 0;
        curRotation.z -= curRotation.z > 180 ? 360 : 0;
        maxRotationDistance = new Vector3(
            Mathf.Abs(desiredTargetRotation.x - curRotation.x),
            Mathf.Abs(desiredTargetRotation.y - curRotation.y),
            Mathf.Abs(desiredTargetRotation.z - curRotation.z)
            );
        Debug.Log(maxRotationDistance);
    }
    public void DragHandler(BaseEventData data)
    {
        // Prevent resetting while dragging
        if (!isDragged)
        {
            startUI = mainCamera.ScreenToWorldPoint(
            new Vector3(transform.position.x,
            transform.position.y,
            mainCamera.WorldToScreenPoint(target.position).z
           ));
            startTarget = target.position;
        }
        isDragged = true;

        // Screen position
        PointerEventData pointerData = (PointerEventData)data;

        // UI Image translate
        transform.position = pointerData.position;

        // Move joint accoding to the pointer iamge
        Vector3 worldPosition = mainCamera.ScreenToWorldPoint(
            new Vector3(pointerData.position.x,
            pointerData.position.y,
            mainCamera.WorldToScreenPoint(target.position).z
           ));
        Vector3 offset = worldPosition - startUI;
        target.position = new Vector3(
            startTarget.x + offset.x,
            startTarget.y + offset.y,
            startTarget.z + offset.z
            );
        EventPool.BodyDragged.Invoke();

    }
    public float getPerformance()
    {
        float performance = 0;
        Vector3 curRotation = joint.localEulerAngles;
        curRotation.x -= curRotation.x > 180 ? 360 : 0;
        curRotation.y -= curRotation.y > 180 ? 360 : 0;
        curRotation.z -= curRotation.z > 180 ? 360 : 0;
        if (match_Z_Rotation)
            performance += Mathf.Max(0, 1 - (Mathf.Abs(curRotation.z - desiredTargetRotation.z) / maxRotationDistance.z));
        if (match_Y_Rotation)
        {
            bool flagDiv = (performance > 0);
            performance += Mathf.Max(0, 1 - (Mathf.Abs(curRotation.y - desiredTargetRotation.y) / maxRotationDistance.y));
            performance /= (flagDiv ? 2 : 1);
        }
        if (match_X_Rotation)
        {
            bool flagDiv = (performance > 0);
            performance += Mathf.Max(0, 1 - (Mathf.Abs(curRotation.x - desiredTargetRotation.x) / maxRotationDistance.x));
            performance /= (flagDiv ? 2 : 1);
        }
        Debug.Log(1 - (Mathf.Abs(curRotation.y - desiredTargetRotation.y) / maxRotationDistance.y));
        Debug.Log(1 - (Mathf.Abs(curRotation.x - desiredTargetRotation.x) / maxRotationDistance.x));
        return performance;
    }

    public void DropHandler(BaseEventData data)
    {
        isDragged = false;
        EventPool.BodyDragged.Invoke();
        ResetPositionOnJoint();
    }
    public void ResetPositionOnJoint()
    {
        if (isDragged)
            return;
        Vector2 screenPos = mainCamera.WorldToScreenPoint(joint.position);
        transform.position = screenPos;
    }
}
