using UnityEngine;

public class Girl : MonoBehaviour
{
    private float percent;
    private bool End;
    private Vector3 bodyStartPosition;
    private Vector3 targetStartPosition;

    [SerializeField]
    private Transform headTarget;

    [Range(0, 7)]
    [SerializeField]
    int levelSelector = 0;
    private void Awake()
    {
        bodyStartPosition = transform.position;
        targetStartPosition = headTarget.position;
    }
    void Start()
    {
        // TODO for later
        if(levelSelector == 1)
            EventPool.finishedFixing.AddListener(ActivateEnd);
    }

    public void ActivateEnd(int percent)
    {
        this.percent = percent;
        End = true;
    }
    void Update()
    {
        if (End)
        {
            Vector3 bodyDestination = new Vector3();
            Vector3 targetDestination = new Vector3();
            if (percent > 75)
            {
                bodyDestination = bodyStartPosition - new Vector3(0.2f, 0, 0);
                targetDestination = targetStartPosition - new Vector3(0.5f, 0, 0);
            }
            else if (percent >= 25)
            {
                bodyDestination = bodyStartPosition;
                targetDestination = targetStartPosition - new Vector3(0.5f, 0, 0);
            }
            else
            {
                bodyDestination = bodyStartPosition + new Vector3(0.2f, 0, 0);
                targetDestination = targetStartPosition + new Vector3(0.5f, 0, 0);
            }
            float bodyDis = Vector3.Distance(transform.position, bodyDestination);
            float targetDis = Vector3.Distance(headTarget.position, targetDestination);
            if (bodyDis > 0.05)
                transform.position = Vector3.Lerp(transform.position, bodyDestination, Time.deltaTime);
            if (targetDis > 0.05)
                headTarget.position = Vector3.Lerp(headTarget.position, targetDestination, 2 * Time.deltaTime);
        }
    }
}
