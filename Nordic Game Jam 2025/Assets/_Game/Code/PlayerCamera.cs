using System;
using Unity.Cinemachine;
using UnityEngine;

public class PlayerCamera : MonoBehaviour
{
    private void Start()
    {
        GetComponent<CinemachineCamera>().Follow = GameObject.Find("CameraOffset").transform;
    }
}
