using System;
using UnityEngine;

public class RespawnBook : MonoBehaviour
{
    public GameObject book;
    Vector3 startPos;
    Vector3 startRot;

    private void Start()
    {
        startPos = book.transform.position;
        startRot = book.transform.rotation.eulerAngles;
    }

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log(other.tag);
        if (other.CompareTag("Book"))
        {
            book.transform.position = startPos;
            book.transform.rotation = Quaternion.Euler(startRot);
            
            book.GetComponent<Rigidbody>().linearVelocity = Vector3.zero;
            book.GetComponent<Rigidbody>().angularVelocity = Vector3.zero;
        }
    }
}
