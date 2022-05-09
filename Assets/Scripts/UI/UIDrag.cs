using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Animations.Rigging;

enum RiggingType
{
    TwoBoneIKConstraint,
    MultiPositionConstraint,
    ChainIKConstraint,

}
public class UIDrag : MonoBehaviour
{
    [TagSelector]
    [SerializeField]
    private string targetTag = "";
    [SerializeField]
    private RiggingType riggingType;
    [SerializeField]
    private bool fixX;
    [SerializeField]
    private bool fixY;
    [SerializeField]
    private bool fixZ;

    private Camera mainCamera; // Used for calculations
    private Transform joint;
    private Transform target;
    private float maxDistance;
    private Vector3 startUI;
    private Vector3 startTarget;
    private bool isDragged = false;
    private Transform desiredGoal;
     // whateverchange
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
        desiredGoal = GameObject.FindGameObjectWithTag(targetTag.Replace("_Target", "_Goal")).transform;
        Debug.Log(desiredGoal);
        if(riggingType == RiggingType.TwoBoneIKConstraint)
            joint = target.transform.parent.GetComponent<TwoBoneIKConstraint>().data.tip;
        else if(riggingType == RiggingType.ChainIKConstraint)
            joint = target.transform.parent.GetComponent<ChainIKConstraint>().data.tip;
        else if (riggingType == RiggingType.MultiPositionConstraint)
            joint = target.transform.parent.GetComponent<MultiPositionConstraint>().data.constrainedObject;
    }

    IEnumerator Start()
    {
        yield return new WaitForEndOfFrame();
        EventPool.BodyDragged.AddListener(ResetPositionOnJoint);
        ResetPositionOnJoint();
        maxDistance = Vector3.Distance(desiredGoal.position, joint.position);
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
            startTarget.x + (fixX ? offset.x : 0f),
            startTarget.y + (fixY ? offset.y : 0f),
            startTarget.z + (fixZ ? offset.z : 0f)
            );
        EventPool.BodyDragged.Invoke();
    }

    public float getPerformance()
    {
        float performance = Mathf.Max(0, 1 - (Vector3.Distance(desiredGoal.position, joint.position) / maxDistance));
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
