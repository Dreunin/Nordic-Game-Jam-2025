using System;
using UnityEngine;

[RequireComponent(typeof(Rigidbody), typeof(Collider))]

public class Pushable : MonoBehaviour
{
    [SerializeField] private bool generatesSound = false;
    [SerializeField] private float maxNoisePerCollision = 1f;
    
    private void Awake()
    {
        throw new NotImplementedException();
    }

    private void OnCollisionEnter(Collision other)
    {
        //Get collision force between this and other
        float collisionForce = other.relativeVelocity.magnitude;

        GameManager.instance.UpdateNoise(Mathf.Clamp(collisionForce,0, maxNoisePerCollision));
    }
}
