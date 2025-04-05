using System;
using UnityEngine;

public class Crown : MonoBehaviour
{
    private Transform _target;
    [SerializeField] float speed = 5f;
    private Vector3 _lastPosition;

    private void Start()
    {
        _target = GameObject.Find("CrownOffset").transform;
        _lastPosition = transform.position;
    }

    private void LateUpdate()
    {
        //Follow target gradually 
        Vector3 targetPosition = _target.position;
        transform.position = Vector3.Lerp(transform.position, targetPosition, Time.deltaTime*speed);
        //Rotate to point at last position
        Vector3 direction = _lastPosition - transform.position;
        if (direction.magnitude < 0.005f)
        {
            //Reset rotation
            transform.rotation = Quaternion.identity;
        }
        else
        {
            Quaternion rotation = Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.Slerp(transform.rotation, rotation, Time.deltaTime * speed);
            //only turn in z axis
            transform.rotation = Quaternion.Euler(0, 0, transform.rotation.eulerAngles.z);
        }
        
        _lastPosition = transform.position;
    }
}
