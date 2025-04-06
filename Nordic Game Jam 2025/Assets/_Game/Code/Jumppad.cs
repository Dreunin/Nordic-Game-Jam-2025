using UnityEngine;

public class Jumppad : MonoBehaviour
{
    public float angle = 35f;
    public float strength = 5000f;
    public bool used = false;

    public void Use()
    {
        used = true;
    }
}
