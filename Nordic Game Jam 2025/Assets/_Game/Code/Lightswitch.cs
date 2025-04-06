using System;
using Unity.VisualScripting;
using UnityEngine;

public class Lightswitch : MonoBehaviour
{
    public bool on = false;
    [SerializeField] private float wakeUpValue = 0.01f;

    private float scale;
    private void Start()
    {
        scale = transform.localScale.x;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            on = !on;
            
            transform.localScale = new Vector3(on ? -scale : scale, scale, scale);
            
            GetComponent<AudioSource>().Play();
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
