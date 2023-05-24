using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class HoverShowName : MonoBehaviour
{
    public TMP_Text nameText;

    // Start is called before the first frame update
    void Start()
    {
        nameText.text = gameObject.name;
    }

    private void OnMouseEnter()
    {
        nameText.gameObject.SetActive(true);
    }

    private void OnMouseExit()
    {
        nameText.gameObject.SetActive(false);
    }
}
