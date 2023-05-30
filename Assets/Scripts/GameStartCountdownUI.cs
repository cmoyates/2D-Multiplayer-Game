using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class GameStartCountdownUI : MonoBehaviour
{
    public TMP_Text countdownText;
    public Image bgColorImage;

    private void Start()
    {
        GameManager.Instance.OnStateChanged += GameManager_OnStateChanged;

        // Hide on start
        gameObject.SetActive(false);
    }

    private void GameManager_OnStateChanged(object sender, System.EventArgs e)
    {
        if (GameManager.Instance.IsCountdownToStartActive())
        {
            // Show
            gameObject.SetActive(true);
        }
        else 
        {
            // Hide
            gameObject.SetActive(false);
        }
    }

    private void Update()
    {
        float countdownTimer = GameManager.Instance.GetCountdownToStartTimer();
        Color bgColor = bgColorImage.color;
        bgColor.a = countdownTimer / 3.0f;
        bgColorImage.color = bgColor;
        countdownText.text = Mathf.CeilToInt(GameManager.Instance.GetCountdownToStartTimer()).ToString();
    }
}
