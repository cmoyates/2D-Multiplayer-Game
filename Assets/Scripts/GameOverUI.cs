using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class GameOverUI : MonoBehaviour
{
    public TMP_Text finalScoreText;

    private void Start()
    {
        GameManager.Instance.OnStateChanged += GameManager_OnStateChanged;

        // Hide on start
        gameObject.SetActive(false);
    }

    private void GameManager_OnStateChanged(object sender, System.EventArgs e)
    {
        if (GameManager.Instance.IsGameOver())
        {
            // Show
            finalScoreText.text = "Final Score: " + PlayerManager.Instance.GetScore().ToString();
            gameObject.SetActive(true);
        }
        else 
        {
            // Hide
            gameObject.SetActive(false);
        }
    }
}
