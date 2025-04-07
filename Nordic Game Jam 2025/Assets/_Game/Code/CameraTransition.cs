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
        if(_cameraLeft == _cameraRight) _cameraLeft.SetActive(true);
        //If left of the object, enable left camera
        if (other.transform.position.x < transform.position.x)
        {
            _cameraRight.SetActive(false);
            _cameraLeft.SetActive(true);
        }
        else
        {
            //If right of the object, enable right camera
            _cameraLeft.SetActive(false);
            _cameraRight.SetActive(true);
        }
    }
}
