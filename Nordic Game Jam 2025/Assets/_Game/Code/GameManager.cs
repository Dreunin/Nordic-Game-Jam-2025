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

    public GameObject gameOverCanvas;

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
        DontDestroyOnLoad(gameObject);
    }

    private void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>();
        noiseText = GameObject.Find("NoiseMade").GetComponent<TextMeshProUGUI>();
        
        //Set default checkpoint to player position
        currentCheckpoint = player.transform.position;
    }
    

    void Update()
    {
        if (noiseMade >= 100)
        {
            LoseGame();
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
        noiseMade = 0;
        noiseText.enabled = true;
    }

    public void UpdateNoise(float noise)
    {
        noiseMade += noise;
        noiseText.text = "Human woken up: " + noiseMade.ToString("0") + "%";
        
        //Press R to restart
        if (Input.GetKeyDown(KeyCode.R))
        {
            ResetToCheckPoint();
        }
    }

    public void ResetToCheckPoint()
    {
        noiseMade = 0;
        ReloadLevel();
        player.ResetVelocity();
        player.transform.position = currentCheckpoint;
        Time.timeScale = 1;
    }

    public void SetCheckpoint(Vector3 position) => currentCheckpoint = position;


    public GameObject wakeUp;
    public CinemachineCamera gameOverCam;
    public void LoseGame()
    {
        noiseMade = 0;
        noiseText.enabled = false;
        //Show animation
        wakeUp.SetActive(true);
        gameOverCam.gameObject.SetActive(true);
        
        //Play sound?
        
        Invoke(nameof(ShowGameOverScreen),4f);
    }
    
    public void ShowGameOverScreen()
    {
        wakeUp.SetActive(false);
        gameOverCam.gameObject.SetActive(false);
        Animator animator = wakeUp.GetComponent<Animator>();
        //Reset animation
        animator.SetTrigger("Reset");
        gameOverCanvas.SetActive(true);
        Time.timeScale = 0;
    }

    public void RestartGame()
    {
        SceneManager.LoadScene("Main_Level");
        noiseText.enabled = true;
        Time.timeScale = 1;
    }

    public void StartExtraction()
    {
        //Destroy player
        noiseText.enabled = false;
        Destroy(player.gameObject);
        //Next scene
        SceneManager.LoadScene("ExtractionScene");
    }
}
