using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CoinScript : PickupScript
{
    [SerializeField]
    bool magnetized = false;
    [SerializeField]
    float magnitizedPullMul = 2.0f;
    float pullTimer = 0;
    [SerializeField]
    Rigidbody2D rb;
    Transform playerTransform;
    Collider2D col;

    // Start is called before the first frame update
    new void Start()
    {
        base.Start();

        rb = GetComponent<Rigidbody2D>();
        col = GetComponent<Collider2D>();

        playerTransform = GameObject.FindGameObjectWithTag("Player").transform;

        LevelManager.Instance.OnRoomLockUnlock += LevelManager_OnRoomLockUnlock;

        if (!LevelManager.Instance.GetIsInLockedRoom())
        {
            magnetized = true;
            col.isTrigger = true;
        }
    }

    private void LevelManager_OnRoomLockUnlock(object sender, System.EventArgs e)
    {
        if (!LevelManager.Instance.GetIsInLockedRoom())
        {
            magnetized = true;
            col.isTrigger = true;
        }
        else
        {
            LevelManager.Instance.OnRoomLockUnlock -= LevelManager_OnRoomLockUnlock;
            Destroy(gameObject);
        }
    }

    private void FixedUpdate()
    {
        if (!magnetized) return;
        pullTimer += Time.fixedDeltaTime;
        rb.AddForce((playerTransform.position - transform.position).normalized * magnitizedPullMul * pullTimer);
    }

    protected override void Interaction()
    {
        pickupSO.Activate(transform.position);
        LevelManager.Instance.OnRoomLockUnlock -= LevelManager_OnRoomLockUnlock;
        Destroy(gameObject);
    }
}
