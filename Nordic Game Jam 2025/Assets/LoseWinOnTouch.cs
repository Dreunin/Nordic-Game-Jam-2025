using System;
using UnityEngine;

public class LoseWinOnTouch : MonoBehaviour
{
    public bool lose = false;
    
    private void OnTriggerEnter(Collider other)
    {
        if(lose)
		{
			GameManager.instance.LoseGame();
		}
        else
        {
	        GameManager.instance.StartExtraction();
        }
	
    }
}
