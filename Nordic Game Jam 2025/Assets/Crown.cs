using System;
using UnityEngine;

public class Crown : MonoBehaviour
{
    private Transform _target;
    [SerializeField] float speed = 5f;

    private void Start()
    {
        _target = GameObject.Find("CrownOffset").transform;
    }

    private void LateUpdate()
    {
        //Follow target gradually 
        Vector3 targetPosition = _target.position;
        transform.position = Vector3.Lerp(transform.position, targetPosition, Time.deltaTime*speed);
    }
}
