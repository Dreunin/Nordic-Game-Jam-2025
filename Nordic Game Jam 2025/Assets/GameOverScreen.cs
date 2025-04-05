using UnityEngine;

public class GameOverScreen : MonoBehaviour
{
    public void Restart()
    {
        GameManager.instance.RestartGame();
    }

    public void ResetToCheckPoint()
    {
        GameManager.instance.ResetToCheckPoint();
    }
}
