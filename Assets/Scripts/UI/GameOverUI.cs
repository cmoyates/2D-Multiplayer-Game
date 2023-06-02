using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameOverUI : MonoBehaviour
{
    public TMP_Text finalScoreText;
    public Button menuButton;
    public Button playAgainButton;

    private void Start()
    {
        GameManager.Instance.OnStateChanged += GameManager_OnStateChanged;

        menuButton.onClick.AddListener(() => {
            SceneManager.LoadScene(0);
        });

        playAgainButton.onClick.AddListener(() => {
            SceneManager.LoadScene(1);
        });

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
