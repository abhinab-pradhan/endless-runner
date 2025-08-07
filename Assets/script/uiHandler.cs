using UnityEngine;
using TMPro;
using System.Collections;
using UnityEngine.SceneManagement;

public class uiHandler : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI distanceTravelText;
    [SerializeField] TextMeshProUGUI gameOverText;
    [SerializeField] CanvasGroup gameOverCanvasGroup;
    carHandler playerCarHandler;
    void Awake()
    {
        playerCarHandler = GameObject.FindGameObjectWithTag("Player").GetComponent<carHandler>();
        playerCarHandler.onPlayerCrashed += playerCarHandler_onPlayerCrashed;
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        gameOverCanvasGroup.interactable = false;
        gameOverCanvasGroup.alpha = 0;
    }

    // Update is called once per frame
    void Update()
    {
        distanceTravelText.text = playerCarHandler.DistanceTravelled.ToString("000000");
    }

    IEnumerator startGameOverAnimation()
    {
        yield return new WaitForSecondsRealtime(3);
        gameOverCanvasGroup.interactable = true;
        while (gameOverCanvasGroup.alpha < .8f)
        {
            gameOverCanvasGroup.alpha = Mathf.MoveTowards(gameOverCanvasGroup.alpha, 1, Time.deltaTime * 2);
            yield return null;
        }
    }

    void playerCarHandler_onPlayerCrashed(carHandler obj)
    {
        gameOverText.text = $"DISTANCE {distanceTravelText.text}";

        StartCoroutine(startGameOverAnimation());
    }

    public void onRestartClicked()
    {
        Time.timeScale = 1;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}
