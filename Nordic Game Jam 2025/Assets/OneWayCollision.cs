using System;
using UnityEngine;
[RequireComponent(typeof(Collider))]
public class OneWayCollision : MonoBehaviour
{
    private Transform playerTransform;
    private void Start()
    {
        playerTransform = GameObject.FindGameObjectWithTag("Player").transform;
    }

    private void Update()
    {
        //If player is above enable collider
        if (playerTransform.position.y > transform.position.y)
        {
            GetComponent<Collider>().enabled = true;
        }
        else
        {
            GetComponent<Collider>().enabled = false;
        }
    }
}
