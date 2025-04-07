using System;
using DG.Tweening;
using UnityEngine;

public class FakeDrawer : MonoBehaviour
{
    public GameObject drawerGO;
    public GameObject climbable;
    private void OnCollisionEnter(Collision other)
    {
        //When the player lands, the drawer will fall out (enable rigidbody 2d on drawerGO)
        if (other.gameObject.CompareTag("Player"))
        {
            drawerGO.AddComponent<Rigidbody2D>();
            //Dotween localrotation x to -45
            drawerGO.transform.DOLocalRotate(new Vector3(-45, 0, 0), 1f);
            Destroy(climbable);
            Destroy(gameObject); //this collider
        }
    }
}
