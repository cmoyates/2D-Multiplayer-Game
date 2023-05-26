using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletScript : MonoBehaviour
{
    Collider2D col;
    // Start is called before the first frame update
    void Start()
    {
        col = GetComponent<Collider2D>();
        StartCoroutine("Lifetime");
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.collider.CompareTag("Enemy"))
        {
            collision.collider.GetComponent<EnemyAI>().Hurt();
        }


        //if (!collision.collider.CompareTag("Player"))
        //{
        //    
        //}

        Destroy(this.gameObject);
    }

    IEnumerator Lifetime() 
    {
        yield return new WaitForSeconds(5);
        Destroy(this.gameObject);
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player") || collision.CompareTag("Enemy")) 
        {
            col.isTrigger = false;
        }
    }
}
