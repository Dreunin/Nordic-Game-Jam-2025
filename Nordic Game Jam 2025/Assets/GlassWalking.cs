using System;
using UnityEngine;

public class GlassWalking : MonoBehaviour
{
    [SerializeField] private float noiseValue = 0.1f;
    [SerializeField] private float inputLimit = 0.5f;
    
    
    
    private void OnTriggerStay(Collider other)
    {
        if (!other.CompareTag("Player")) return;
        float input = Mathf.Abs(other.GetComponent<PlayerController>().GatherInput().x);
        if (input < inputLimit) return;
        GameManager.instance.UpdateNoise(noiseValue*Time.deltaTime*input);
    }
}
