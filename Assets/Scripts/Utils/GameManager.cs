using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    private int percent = 0;
    private List<UIDrag> draggers;
    private List<UIRotate> rotators;
    private Bar bar;

    [Range(0f, 1f)]
    [SerializeField]
    private float bonusMulitplier = 0.04f;
    public static GameManager Instance { get; private set; }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this.gameObject);
            return;
        }
        bar = FindObjectOfType<Bar>();
        draggers = new List<UIDrag>(FindObjectsOfType<UIDrag>());
        rotators = new List<UIRotate>(FindObjectsOfType<UIRotate>());
        Debug.Log(draggers.Count + rotators.Count);
        Instance = this;
    }
    private void Start()
    {
        EventPool.BodyDragged.AddListener(CheckPerformance);
        TinySauce.OnGameStarted(SceneManager.GetActiveScene().buildIndex.ToString());
    }
    private void CheckPerformance()
    {
        int numberOfJoints = draggers.Count;
        int numberOfRotators = rotators.Count;

        float max = numberOfJoints + numberOfRotators;

        float performance = 0;

        foreach (UIDrag dragScript in draggers)
        {
            float curPerformance = dragScript.getPerformance();
            performance += curPerformance;
        }

        foreach (UIRotate rotatorsScript in rotators)
        {
            float curPerformance = rotatorsScript.getPerformance();
            performance += curPerformance;
        }
        performance += bonusMulitplier * max;
        performance /= max;
        Debug.Log("Final " + performance);
        if (performance > 0.90)
            percent = 100;
        else if (performance > 0.70f)
            percent = 75;
        else if (performance > 0.43f)
            percent = 50;
        else if (performance > 0.12f)
            percent = 25;
        else
            percent = 0;
        bar.SetBar(percent);
        //Debug.Log(percent + "%");
    }
    public void Finish()
    {
        EventPool.finishedFixing.Invoke(percent);
        TinySauce.OnGameFinished(true, percent, SceneManager.GetActiveScene().buildIndex.ToString());
    }
}


