using System;
using UnityEngine;

public class CheeseTrap : MonoBehaviour
{
    private Rigidbody playerRB;
    [SerializeField] private float cheeseLureStrength = 1f;
    [SerializeField] private float maxPull = 1000f;

    private void Start()
    {
        playerRB = GameObject.FindGameObjectWithTag("Player").GetComponent<Rigidbody>();
    }

    private void OnTriggerStay(Collider other)
    {
        //If player
        if(other.CompareTag("Player"))
        {
            //Add player force proportionally to distance between player and cheese
            Vector3 direction = transform.position - other.transform.position;
            float distance = direction.magnitude;
            
            Vector3 directionalForce = direction.normalized * Mathf.Clamp(1/distance * cheeseLureStrength,-maxPull,maxPull);
            playerRB.AddForce(directionalForce);
            Debug.Log(directionalForce);
        }
    }
}
