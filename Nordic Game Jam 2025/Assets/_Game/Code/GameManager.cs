using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    float noiseMade; //If this reaches 100, you lose
    public static GameManager instance;
    [SerializeField] TextMeshProUGUI noiseText;
    
    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }
    
    void Update()
    {
        if (noiseMade >= 100)
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }
    }

    public void UpdateNoise(float noise)
    {
        noiseMade += noise;
        noiseText.text = "Noise: " + noiseMade.ToString("0.00");
    }
}
