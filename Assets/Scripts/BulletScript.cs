using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletScript : MonoBehaviour
{
    Collider2D col;
    [SerializeField]
    bool isEnemyBullet = false;

    // Start is called before the first frame update
    void Start()
    {
        GameManager.Instance.OnStateChanged += GameManager_OnStateChanged;

        col = GetComponent<Collider2D>();
    }

    private void GameManager_OnStateChanged(object sender, System.EventArgs e)
    {
        if (GameManager.Instance.IsGameOver())
        {
            Destroy(this.gameObject);
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (isEnemyBullet)
        {
            if (collision.collider.CompareTag("Player"))
            {
                collision.collider.GetComponent<PlayerController>().Hurt(1);
            }
        }
        else 
        {
            if (collision.collider.CompareTag("Enemy"))
            {
                collision.collider.GetComponent<AIBase>().Hurt();
            }
        }

        Destroy(this.gameObject);
    }

    private void OnDestroy()
    {
        GameManager.Instance.OnStateChanged -= GameManager_OnStateChanged;
    }


    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player") || collision.CompareTag("Enemy")) 
        {
            col.isTrigger = false;
        }
    }
}
