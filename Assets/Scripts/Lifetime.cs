using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Lifetime : MonoBehaviour
{
    [SerializeField]
    float lifetime = 5.0f;

    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine("LifetimeCoroutine");
    }

    IEnumerator LifetimeCoroutine()
    {
        yield return new WaitForSeconds(5);
        
        Destroy(this.gameObject);
    }
}
