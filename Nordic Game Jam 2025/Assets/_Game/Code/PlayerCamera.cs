using System;
using Unity.Cinemachine;
using UnityEngine;

public class PlayerCamera : MonoBehaviour
{
    private void Start()
    {
        GetComponent<CinemachineCamera>().Follow = GameObject.FindGameObjectWithTag("Player").transform;
    }
}
