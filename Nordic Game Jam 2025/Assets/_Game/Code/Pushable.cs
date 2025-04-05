using System;
using UnityEngine;

[RequireComponent(typeof(Rigidbody), typeof(Collider))]

public class Pushable : MonoBehaviour
{
    [SerializeField] private bool generatesSound = false;
    [SerializeField] private float maxNoisePerCollision = 1f;
    Rigidbody rb;
    
    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        rb.constraints = RigidbodyConstraints.FreezeRotation;
    }

    private void OnCollisionEnter(Collision other)
    {
        //Get collision force between this and other
        float collisionForce = other.relativeVelocity.magnitude;

        GameManager.instance.UpdateNoise(Mathf.Clamp(collisionForce,0, maxNoisePerCollision));
    }
}
