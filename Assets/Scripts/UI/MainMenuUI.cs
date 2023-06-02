using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MainMenuUI : MonoBehaviour
{
    public Button playButton;
    public float transitionTimer = 1.0f;
    float currentTransitionTimer;
    bool fading = false;
    public Image transitionFadeImage;

    private void Awake()
    {
        playButton.onClick.AddListener(() => {
            fading = true;
            transitionFadeImage.gameObject.SetActive(true);
        });

        currentTransitionTimer = transitionTimer;
    }

    private void Update()
    {
        // Do nothing if not fading to black
        if (!fading) return;

        // Decrease the timer every frame
        currentTransitionTimer -= Time.deltaTime;
        // Get the current color of the fade
        Color bgColor = transitionFadeImage.color;
        // Set the opacity according to the percentage of the timer elapsed
        bgColor.a = 1.0f - (currentTransitionTimer / transitionTimer);
        // Set the fade to the new color
        transitionFadeImage.color = bgColor;

        // If the screen is completely black
        if (bgColor.a >= 1.0f) 
        {
            // The fading has stopped
            fading = false;
            // Load the scene
            SceneManager.LoadScene(1);
        }
    }
}
