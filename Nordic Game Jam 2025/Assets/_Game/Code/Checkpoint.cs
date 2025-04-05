using System;
using UnityEngine;

public class Checkpoint : MonoBehaviour
{
    private void OnTriggerExit(Collider other)
    {
        GameManager.instance.SetCheckpoint(transform.position);
    }
}
