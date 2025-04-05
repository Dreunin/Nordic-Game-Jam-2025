using System;
using Unity.VisualScripting;
using UnityEngine;

public class Lightswitch : MonoBehaviour
{
    public bool on = false;
    [SerializeField] private float wakeUpValue = 0.01f;
    private void OnTriggerEnter(Collider other)
    {
        
        if (other.CompareTag("Player"))
        {
            on = !on;
        }
    }

    private void Update()
    {
        if (on)
        {
            GameManager.instance.UpdateNoise(wakeUpValue * Time.deltaTime);
        }
    }
}
