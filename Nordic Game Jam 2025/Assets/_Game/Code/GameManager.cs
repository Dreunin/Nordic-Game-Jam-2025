using System;
using TMPro;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    float noiseMade; //If this reaches 100, you lose
    public static GameManager instance;
    TextMeshProUGUI noiseText;
    private Vector3 currentCheckpoint;

    private PlayerController player;
    
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

    private void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>();
        noiseText = GameObject.Find("NoiseMade").GetComponent<TextMeshProUGUI>();
    }

    void Update()
    {
        if (noiseMade >= 100)
        {
            ReloadLevel();
        }
        
        //On pressing R, reset to checkpoint
        if (Input.GetKeyDown(KeyCode.R))
        {
            ResetToCheckPoint();
        }
    }

    private void ReloadLevel()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void UpdateNoise(float noise)
    {
        noiseMade += noise;
        noiseText.text = "Noise: " + noiseMade.ToString("0.00");
        
        //Press R to restart
        if (Input.GetKeyDown(KeyCode.R))
        {
            ResetToCheckPoint();
        }
    }

    public void ResetToCheckPoint()
    {
        ReloadLevel();
        player.ResetVelocity();
        player.transform.position = currentCheckpoint;
    }

    public void SetCheckpoint(Vector3 position) => currentCheckpoint = position;

    public void LoseGame()
    {
        //Play losing animation
        
        //Wait for animation to finish
        //Reload level
        ResetToCheckPoint();
    }

    public void RestartGame()
    {
        SceneManager.LoadScene(0);
    }
}
