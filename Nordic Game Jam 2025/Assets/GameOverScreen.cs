using UnityEngine;

public class GameOverScreen : MonoBehaviour
{
    public void Restart()
    {
        GameManager.instance.RestartGame();
    }

    public void ResetToCheckPoint()
    {
        gameObject.SetActive(false);
        GameManager.instance.ResetToCheckPoint();
    }
    
    public void Update()
    {
        if (Input.anyKeyDown)
        {
            ResetToCheckPoint();
        }
    }
}
