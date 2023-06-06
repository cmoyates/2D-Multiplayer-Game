using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class GamePlayUI : MonoBehaviour
{
    public TMP_Text scoreText;
    Vector3 endRoomPos = Vector3.zero;
    Transform playerTransform;
    [SerializeField]
    Image compassImage;
    [SerializeField]
    Sprite fullHeartSprite;
    [SerializeField]
    Sprite emptyHeartSprite;
    [SerializeField]
    Transform heartParent;
    [SerializeField]
    GameObject heartPrefab;
    int playerMaxHealth = -1;

    // Start is called before the first frame update
    void Start()
    {
        GameManager.Instance.OnStateChanged += GameManager_OnStateChanged;
        
        PlayerManager.Instance.OnHealthChanged += PlayerManager_OnHealthChanged;
        PlayerManager.Instance.OnScoreChanged += PlayerManager_OnScoreChanged;

        playerMaxHealth = PlayerManager.Instance.GetMaxHealth();

        scoreText.text = "Score: 0";

        playerTransform = GameObject.FindGameObjectWithTag("Player").transform;

        for (int i = 0; i < PlayerManager.Instance.GetMaxHealth(); i++)
        {
            Instantiate(heartPrefab, heartParent);
        }

        // Hide on start
        gameObject.SetActive(false);
    }

    private void Update()
    {
        Vector3 dir = endRoomPos - playerTransform.position;
        float angle = Vector3.Angle(Vector3.up, dir);
        compassImage.rectTransform.rotation = Quaternion.Euler(0f, 0f, angle);
    }

    private void GameManager_OnStateChanged(object sender, System.EventArgs e)
    {
        if (GameManager.Instance.IsGamePlaying())
        {
            endRoomPos = LevelManager.Instance.endRoom.transform.position;
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
        int newPlayerMaxHealth = PlayerManager.Instance.GetMaxHealth();
        int newPlayerHealth = PlayerManager.Instance.GetHealth();
        if (newPlayerMaxHealth > playerMaxHealth)
        {
            for (int i = 0; i < newPlayerMaxHealth - playerMaxHealth; i++) 
            {
                Instantiate(heartPrefab, heartParent);
            } 
        }
        else if (newPlayerMaxHealth < playerMaxHealth) 
        {
            for (int i = 0; i < playerMaxHealth - newPlayerHealth; i++)
            {
                Destroy(heartParent.GetChild(i));
            }
        }
        for (int i = 0; i < heartParent.childCount; i++)
        {
            heartParent.GetChild(i).GetComponent<Image>().sprite = (i >= newPlayerHealth) ? emptyHeartSprite : fullHeartSprite; 
        }
        playerMaxHealth = newPlayerMaxHealth;
    }
}
