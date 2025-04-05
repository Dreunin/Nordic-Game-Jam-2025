using System;
using UnityEngine;

public class CheeseTrap : MonoBehaviour
{
    private Rigidbody playerRB;
    [SerializeField] private float cheeseLureStrength = 1f;
    [SerializeField] private float maxPull = 1000f;
    
    [SerializeField] AudioSource audioSource;

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
            
            //Scale audio volume based on distance
            audioSource.volume = Mathf.Clamp((1/distance)*2,0,1);
        }
    }
}
