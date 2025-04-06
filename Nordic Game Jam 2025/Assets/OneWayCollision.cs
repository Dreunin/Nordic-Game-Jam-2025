using System;
using UnityEngine;
[RequireComponent(typeof(Collider))]
public class OneWayCollision : MonoBehaviour
{
    private Transform playerTransform;
    private CapsuleCollider playerCollider;
    private void Start()
    {
        playerTransform = GameObject.FindGameObjectWithTag("Player").transform;
        playerCollider = playerTransform.GetComponent<CapsuleCollider>();
    }

    private void Update()
    {
        //If player is above enable collider
        if (playerTransform.position.y > transform.position.y + (playerCollider.height/2)*0.95f)
        {
            GetComponent<Collider>().enabled = true;
        }
        else
        {
            GetComponent<Collider>().enabled = false;
        }
    }
}
