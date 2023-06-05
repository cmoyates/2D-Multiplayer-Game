using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class LevelGenRect : MonoBehaviour
{
    [SerializeField]
    HashSet<Transform> overlappingRects = new HashSet<Transform>();
    Rigidbody2D rb;
    public LevelGenerator lg;
    public int index;
    bool stopped = false;
    [SerializeField]
    BoxCollider2D collsionCollider;
    [SerializeField]
    BoxCollider2D triggerCollider;
    [SerializeField]
    Light2D roomLight;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void FixedUpdate()
    {
        if (stopped) return;


        if (overlappingRects.Count == 0)
        {
            rb.velocity = Vector2.zero;
        }
        else 
        {
            Vector2 separationVec = Vector2.zero;
            foreach (var rect in overlappingRects)
            {
                separationVec += (Vector2)(rect.position - transform.position);
            }
            rb.velocity = -separationVec * 3;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Debug.Log("Enter: " + name + " : " + collision.gameObject.name);
        overlappingRects.Add(collision.transform);
        lg.RectOverlapChange(index, true);
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        // Debug.Log("Exit: " + name + " : " + collision.gameObject.name);
        overlappingRects.Remove(collision.transform);
        lg.RectOverlapChange(index, overlappingRects.Count != 0);
    }

    public void Stop() 
    {
        stopped = true;
        rb.velocity = Vector2.zero;
        Destroy(triggerCollider);
        Destroy(rb);
    }

    public void SetSize(Vector2Int size, int gap) 
    {
        collsionCollider.size = size;
        triggerCollider.size = size + Vector2Int.one * gap * 2;
        roomLight.pointLightOuterRadius = Mathf.Max(size.x, size.y);
    }
}
