using System;
using DG.Tweening;
using UnityEngine;
using Random = UnityEngine.Random;

public class Tooth : MonoBehaviour
{
    private bool _collected;
    private Collider2D _collider;
    
    public void Awake()
    {
        _collider = GetComponent<Collider2D>();
    }

    public void Collect()
    {
        _collected = true;
        _collider.enabled = false;
        transform.parent = null;
        
        GetComponent<SpriteRenderer>().sortingOrder = 50;
        
        // Add rigidbody
        Rigidbody2D rb = gameObject.AddComponent<Rigidbody2D>();
        float randomAngle = Random.Range(Mathf.PI / 6, Mathf.PI / 6 * 5) - Mathf.PI / 2;
        float randomForce = Random.Range(200f, 400f);
        float randomAngularVelocity = Random.Range(100f, 600f);
        rb.AddForce(new Vector2(Mathf.Sin(randomAngle), Mathf.Cos(randomAngle)) * randomForce);
        rb.angularVelocity = randomAngularVelocity;
        
        // Punch the scalc
        transform.DOPunchScale(new Vector3(0.3f, 0.3f, 0.3f), 1, 1);
    }

    public bool IsCollected()
    {
        return _collected;
    }
}
