using System;
using Unity.VisualScripting;
using UnityEngine;

public class Lightswitch : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            GameManager.instance.LoseGame();
        }
    }
}
