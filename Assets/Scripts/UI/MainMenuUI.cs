using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MainMenuUI : MonoBehaviour
{
    public Button playButton;

    private void Awake()
    {
        playButton.onClick.AddListener(() => {
            SceneManager.LoadScene(1);
        });
    }
}
