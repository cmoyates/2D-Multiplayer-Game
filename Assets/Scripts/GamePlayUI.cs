using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class GamePlayUI : MonoBehaviour
{
    public TMP_Text healthText;
    public TMP_Text scoreText;

    // Start is called before the first frame update
    void Start()
    {
        GameManager.Instance.OnStateChanged += GameManager_OnStateChanged;
        
        PlayerManager.Instance.OnHealthChanged += PlayerManager_OnHealthChanged;
        PlayerManager.Instance.OnScoreChanged += PlayerManager_OnScoreChanged;

        healthText.text = "Health: " + PlayerManager.Instance.GetHealth() + "/" + PlayerManager.Instance.GetMaxHealth();
        scoreText.text = "Score: 0";


        // Hide on start
        gameObject.SetActive(false);
    }

    private void GameManager_OnStateChanged(object sender, System.EventArgs e)
    {
        if (GameManager.Instance.IsGamePlaying())
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

    private void PlayerManager_OnScoreChanged(object sender, System.EventArgs e)
    {
        scoreText.text = "Score: " + PlayerManager.Instance.GetScore().ToString();
    }

    private void PlayerManager_OnHealthChanged(object sender, System.EventArgs e)
    {
        healthText.text = "Health: " + PlayerManager.Instance.GetHealth() + "/" + PlayerManager.Instance.GetMaxHealth();
    }
}
