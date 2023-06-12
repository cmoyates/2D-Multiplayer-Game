using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Lifetime : MonoBehaviour
{
    [SerializeField]
    float lifetime = 5.0f;
    float currentLifetime = 0;

    private void Update()
    {
        currentLifetime += Time.deltaTime;
        if (currentLifetime >= lifetime) 
        {
            Destroy(this.gameObject);
        }
    }
}
