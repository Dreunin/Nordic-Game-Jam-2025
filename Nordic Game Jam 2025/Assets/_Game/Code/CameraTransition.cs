using System;
using UnityEngine;

public class CameraTransition : MonoBehaviour
{
    [SerializeField] private GameObject _cameraLeft;
    [SerializeField] private GameObject _cameraRight;

    private void Awake()
    {
        GetComponent<MeshRenderer>().enabled = false;
    }

    private void OnTriggerExit(Collider other)
    {
        if (!other.gameObject.CompareTag("Player")) return;
        //If left of the object, enable left camera
        if (other.transform.position.x < transform.position.x)
        {
            _cameraLeft.SetActive(true);
            _cameraRight.SetActive(false);
        }
        else
        {
            //If right of the object, enable right camera
            _cameraLeft.SetActive(false);
            _cameraRight.SetActive(true);
        }
    }
}
