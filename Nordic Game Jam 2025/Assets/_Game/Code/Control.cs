using UnityEngine;
using UnityEngine.SceneManagement;

public class Control : MonoBehaviour
{
    public void NextScene()
    {
        SceneManager.LoadScene("Main_Level");
    }

    public void Update()
    {
        if (Input.anyKeyDown)
        {
            NextScene();
        }
    }
}