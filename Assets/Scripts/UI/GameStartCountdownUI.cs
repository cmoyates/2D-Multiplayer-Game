using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class GameStartCountdownUI : MonoBehaviour
{
    public TMP_Text countdownText;
    public Image bgColorImage;
    bool waitingToStart = true;

    private void Start()
    {
        GameManager.Instance.OnStateChanged += GameManager_OnStateChanged;

        // Hide on start
        gameObject.SetActive(false);

        GameManager_OnStateChanged(null, null);
    }

    private void GameManager_OnStateChanged(object sender, System.EventArgs e)
    {
        waitingToStart = GameManager.Instance.IsWaitingToStart();
        if (GameManager.Instance.IsCountdownToStartActive() || waitingToStart)
        {
            // Show
            gameObject.SetActive(true);
            countdownText.text = waitingToStart ? "Loading..." : "3";
        }
        else 
        {
            // Hide
            gameObject.SetActive(false);
        }
    }

    private void Update()
    {
        if (waitingToStart) return;
        float countdownTimer = GameManager.Instance.GetCountdownToStartTimer();
        Color bgColor = bgColorImage.color;
        bgColor.a = countdownTimer / 3.0f;
        bgColorImage.color = bgColor;
        countdownText.text = Mathf.CeilToInt(GameManager.Instance.GetCountdownToStartTimer()).ToString();
    }
}
