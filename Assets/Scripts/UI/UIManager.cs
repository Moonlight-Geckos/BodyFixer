using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public class UIManager : MonoBehaviour
{
    private Animator animator;
    private Transform startGameText;
    private Transform handIndicator;
    private Transform finishButton;
    private bool isClicking = false;
    private void Awake()
    {
        animator = GetComponent<Animator>();
        Transform inGamePanel = transform.GetChild(0);
        startGameText = inGamePanel.Find("StartGameText");
        handIndicator = inGamePanel.Find("HandIndicator");
        finishButton = inGamePanel.Find("FinishButton");
        handIndicator.gameObject.SetActive(false);
    }
    private void Start()
    {
        EventPool.finishedFixing.AddListener(PlayUIAnimation);
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            if (startGameText.gameObject.activeSelf)
                startGameText.gameObject.SetActive(false);
            isClicking = true;
            handIndicator.gameObject.SetActive(true);
        }
        else if (Input.GetMouseButtonUp(0))
        {
            isClicking = false;
            handIndicator.gameObject.SetActive(false);
        }
        if (isClicking)
        {
            handIndicator.transform.position = Input.mousePosition;
        }
    }
    private async void PlayUIAnimation(int precent)
    {
        await System.Threading.Tasks.Task.Delay(2000);
        animator.SetInteger("Percent", precent);
    }
    public void Restart()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
    public void NextScene()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }
    public void Finish()
    {
        finishButton.gameObject.SetActive(false);
        GameManager.Instance.Finish();
    }
}
